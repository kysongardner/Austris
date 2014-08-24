using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Configuration;
using System.IO;
using Wpf_tetris.engine;
namespace Wpf_tetris
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        SortedList<Double, PlayerScore> TopScores = new SortedList<Double, PlayerScore>();
        PlayerScore NewScore = new PlayerScore("");
        TetrisGameBoard tgb ;
        Random rnd = new Random();
        SolidColorBrush ScoreColorBrush = new SolidColorBrush(Colors.Red);
        int oldScore = 0;
        OnlineScores.OnlineScoresSoapClient ossc = null;
        badwords bw = new badwords();
        System.Threading.Thread webThread;
        public Window1()
        {
            InitializeComponent();
            ossc = new Wpf_tetris.OnlineScores.OnlineScoresSoapClient();//.OnlineScoresSoapClient("http://allegiancestats.com/austrisscores/OnlineScores.asmx");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtNewPoints.Text = "";
            webThread = new System.Threading.Thread(new System.Threading.ThreadStart(  FetchAndUpdateOnlineScores));
            webThread.Start();
            startNewGame();
            txtNewPoints.Foreground = ScoreColorBrush;
        }

        private void startNewGame()
        {
            txtGameOver.Visibility = Visibility.Hidden;
            TopScores.Clear();

            if (tgb != null)
            {
                tgb.Pause();
                tgb = null;
            }

            List<Shape> toRemove =  canvas1.Children.OfType<Shape>().ToList();

            foreach (Shape e in toRemove)
            {
                canvas1.Children.Remove(e);
            }
           
            

            tgb = new TetrisGameBoard(this.canvas1, rnd,(Brush) this.Resources["BoarderBrush"]);
            tgb.ScoreChangedEvent += new TetrisGameBoard.ScoreChangedEventHandler(tgb_ScoreChangedEvent);
            //displayBoard(sender, e);
            //displaySetShapes(sender, e);
            // and download the scores from the server.
            
            displayHighScores();
            if (webThread.ThreadState != System.Threading.ThreadState.Stopped)
            { txtHighScores.Text = "Loading Scores... " + Environment.NewLine; }
        }

        private void FetchAndUpdateOnlineScores()
        {
            List<PlayerScore> myOnlinePS = new List<PlayerScore>();

            bool hitserver = false;
            try
            {
                OnlineScores.ArrayOfString myOnlineScores = ossc.GetScores();
                // lets split up our online scores into something nice

                foreach (var item in myOnlineScores)
                {
                    string[] names = item.Split(new char[] { ':' });
                    string name = names[0].Replace("name=\"", "").Replace("\"", "");
                    string level = names[1].Replace("level=\"", "").Replace("\"", "");
                    string points = names[2].Replace("points=\"", "").Replace("\"", "");
                    PlayerScore ps = new PlayerScore(string.Format("{0}?{1}?{2}", level, points, name));
                    myOnlinePS.Add(ps);
                }
                hitserver = true;
            }
            catch (Exception ex)
            {
                // for some reason we didn't hit the scores server.
            }
            if (Properties.Settings.Default.HighScores != null)
            {
                string val = Properties.Settings.Default.HighScores;// .Properties["HighScoresSetting"].DefaultValue as string;

                string[] scores = val.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < scores.Length; i++)
                {
                    PlayerScore ps = new PlayerScore(scores[i]);
                    Double rank = getRank(ps.Level, ps.Points);
                    if (!TopScores.ContainsKey(rank))
                        TopScores.Add(rank, ps);
                }
                // lets upload the scores we have that arnt on the server.
                foreach (var item in TopScores)
                {
                    bool found = false;
                    foreach (var ei in myOnlinePS)
                    {
                        if (ei.Name == item.Value.Name && ei.Level == item.Value.Level && ei.Points == item.Value.Points)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found == false && hitserver)
                    {

                        ossc.PutScore(item.Value.Name, item.Value.Level, item.Value.Points);
                    }
                }
                // now lets add scores to topScores that are not already in it.
                foreach (var item in myOnlinePS)
                {
                    Double rank = getRank(item.Level, item.Points);
                    if (!TopScores.ContainsKey(rank))
                        TopScores.Add(rank, item);
                }

            }
            
            txtHighScores.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(displayHighScores));
        }

        void displayHighScores()
        {           

            UpdateZOrder(txtHighScores, true);
            txtHighScores.Text = "Name  lvl Points " + Environment.NewLine;
            
            foreach (KeyValuePair<double,PlayerScore> score in TopScores.Reverse().Take(35))
            {
                string name = score.Value.Name.Length > 8 ? score.Value.Name.Substring(0, 8) : score.Value.Name;
                name = name.PadRight(8);
                txtHighScores.Text += string.Format( "{0} {1}  {2}" + Environment.NewLine, name,score.Value.Level,score.Value.Points);
            } 
        }

        double getRank(int Level, int Points)
        {

            string pts = Points.ToString();
            pts = pts.PadLeft(6, '0');

            return Convert.ToDouble(string.Format("{0}.{1}", Level, pts));
        }
     
        string serializeTopScores(SortedList<Double, PlayerScore> topScores)
        {
            string result = "";
            foreach (KeyValuePair<double,PlayerScore> score in topScores)
            {
                result += score.Value.serialize();
            }
            return result;
        }
        
        void tgb_ScoreChangedEvent(ScoreChangedEventArgs args)
        {
            txtScore.Text = string.Format("Score {0} Level {1} " + Environment.NewLine + "Lines {2} ", args.Score, args.Level, args.Lines);

            if (oldScore != args.Score)
            {
                int howmanyNewPoints = args.Score - oldScore;
                txtNewPoints.Text = howmanyNewPoints.ToString();
                
                ColorAnimation ForeColorAnim = new ColorAnimation(Colors.Black,Colors.Red,TimeSpan.FromMilliseconds(500));
                ForeColorAnim.AutoReverse = true;
                ScoreColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, ForeColorAnim);

                DoubleAnimation fontSizeAnim = new DoubleAnimation(8, 36, TimeSpan.FromMilliseconds(500));
                fontSizeAnim.AutoReverse = true;
                txtNewPoints.BeginAnimation(TextBlock.FontSizeProperty, fontSizeAnim);

                DoubleAnimation rightPosition = new DoubleAnimation(10, 30, TimeSpan.FromMilliseconds(500));
                rightPosition.AutoReverse = true;
                txtNewPoints.BeginAnimation(Canvas.RightProperty, rightPosition);

            }

            oldScore = args.Score;
            testGameOver(args);
        }

        private void testGameOver(ScoreChangedEventArgs args)
        {
            if (args.gameOver == true)
            {
                tgb.Pause();
                tgb.ScoreChangedEvent -= new TetrisGameBoard.ScoreChangedEventHandler(tgb_ScoreChangedEvent);

                NewScore.Level = args.Level;
                NewScore.Points = args.Score;

                txtName.Visibility = Visibility.Visible;
                txtName.Focus();
                UpdateZOrder(txtName, true);
                txtName.SelectAll();


                double newrank = getRank(NewScore.Level, NewScore.Points);
                int rankindex = getRankIndex(newrank, TopScores);
                if (!TopScores.ContainsKey(newrank))
                {
                    txtGameOver.Text = getGameOverText(rankindex);

                }
                else
                {
                    txtGameOver.Text = string.Format("Ohh your too slow! Someone already got {0} points on level {1}.", NewScore.Points, NewScore.Level);
                }

                txtGameOver.Visibility = Visibility.Visible;
                txtGameOver.Background = new SolidColorBrush(Colors.Black);

                displayHighScores();
                UpdateZOrder(txtGameOver, true);
            }
        }

        private string getGameOverText(int rankindex)
        {
            SortedList<int, string> goodList = new SortedList<int, string>();
            goodList.Add(1, "Don't look back, they might be gaining on you.");
            goodList.Add(2, "So close!! Too Bad that Close Only Counts in Horseshoes…and Hand Grenades.");
            goodList.Add(3, "Been there, done that, got the T-shirt.");
            goodList.Add(4, "Every solution breeds new problems.");
            goodList.Add(5, "Madness takes its toll.");
            goodList.Add(6, "Peace, Love, and Prosperity !");
            goodList.Add(7, "Rank has its privileges.");
            goodList.Add(8, "Among the lucky, you are the chosen one.");
            goodList.Add(9, "Can I tell you something? You have the prettiest eyes I have ever seen!");
            goodList.Add(10, "I can tell you anything.");
            goodList.Add(11, "You are looking really good. I don’t think I’ve seen you any skinnier.");
            goodList.Add(12, "You’re the hottest girl in the room.");
            goodList.Add(13, "You make me a better person.");
            goodList.Add(14, "You know, you’re gorgeous.");
            goodList.Add(15, "You smell great.");
            List<string> badList = new List<string>();
            badList.Add("You are an excellent candidate for our total makeover show. Would you like to try out?");
            badList.Add("I like your hair, especially the color at the roots.");
            badList.Add("You’re hot like your sister. Is she single?");
            badList.Add("You’ve got a face only a mother could love.");
            badList.Add("You’re cute, you know that? You look just like my ex-boyfriend");
            badList.Add("I was such an ugly kid...When I played in the sandbox, the cat kept covering me up.");
            badList.Add("I could tell my parents hated me. My bath toys were a toaster and radio.");
            badList.Add("I'm so ugly...My father carried around a picture of the kid who came with his wallet.");
            badList.Add("I remember the time that I was kidnapped and they sent a piece of my finger to my father. He said he wanted more proof.");
            badList.Add("One year they wanted to make me poster boy for birth control.");
            badList.Add("I'm so ugly, when I was born the doctor slapped my mother!");
            badList.Add("One day I came home early from work ... I saw a guy jogging naked. I said to the guy, \"Hey buddy, why are you doing that?\" He said \"Because you came home early.\"");
            badList.Add("I'm so ugly...My mother had morning sickness...AFTER I was born.");
            badList.Add("Daddy, what does FORMATTING DRIVE C: mean?");
            badList.Add("Happiness is a warm puppy, said the anaconda.");
            badList.Add("After all is said and done, usually more is said.");
            badList.Add("An effective way to deal with predators is to taste terrible.");
            badList.Add("Badness comes in waves.");
            badList.Add("Be seeing you.");
            badList.Add("Be happy with the real pleasures in life.");
            badList.Add("Boredom reigns.");
            badList.Add("Celibacy is hereditary.");
            badList.Add("Don't force it; use a hammer.");
            badList.Add("Genetics:  Why you look like your father, or if you don't, why you should.");
             badList.Add("Fast, Cheap, Good:  Choose any two.");
                 badList.Add("Few desires, happy life.");
                 badList.Add("File not found. Should I fake it? (Y/N)");
                 badList.Add("Flee at once, all is discovered.");
                 badList.Add("For a good time, call 452-6089.");
                 badList.Add("For he who builds his casbah out of halvah, beware the nibblers.");
                 badList.Add("For people who like peace and quiet: a phoneless cord.");
                 badList.Add("For some good grease call 921-7723. (Hoagie Haven)");
                 badList.Add("For the sake of one rose, the gardener becomes the servant of a thousand swords.");
                 badList.Add("Four out of the five dentists I surveyed recommended root canals for patients.");
                 badList.Add("Function reject.");
                 badList.Add("Genderplex: Trying to determine from the cutesy pictures which restroom to use.");
                 badList.Add("Generally you don't see that kind of behavior in a major appliance.");
                 badList.Add("Generosity and perfection are your everlasting goals.");
                 badList.Add("Genetics:  Why you look like your father, or if you don't, why you should.");
                 badList.Add("Gentleman:  Knows how to play the bagpipes, but doesn't.");
                 badList.Add("Get out the Crisco.");
                 badList.Add("Ghosts can't rule. You'd never get the crown to stay on.");
                 badList.Add("Give a skeptic an inch and he'll measure it.");
                 badList.Add("Give him an evasive answer.");
                 badList.Add("Give me all your lupins!");
                 badList.Add("Give me chastity and continence, but not just now.  -- St. Augustine");
                 badList.Add("Give up.");
                 badList.Add("Given my druthers, I'd druther not.");
                 badList.Add("Go away.");
                 badList.Add("Go directly to jail. Do not pass Go, do not collect $200.");
                 badList.Add("God does not play dice.");
                 badList.Add("God made the integers; all else is the work of Man.");
                 badList.Add("God never imposes a duty upon us without giving us time and strength to perform it.");
                 badList.Add("God never misses an opportunity--if the door is open he will come in.");
                   badList.Add("Good judgement comes from experience. Experience comes from bad judgement.");
                 badList.Add("Good news from afar can bring you a welcome visitor.");
                 badList.Add("Gossip: letting the chat out of the bag.");
                 badList.Add("Grasshoppotamus:  A creature that can leap to tremendous heights...  once.");
                 badList.Add("Great minds run in great circles.");
                 badList.Add("Had there been an actual emergency, you would no longer be here.");
                 badList.Add("Hailing frequencies open, Captain.");
                 badList.Add("Handel was half German, half Italian, and half English.  He was rather large.");
                 badList.Add("Happy feast of the pig.");
                 badList.Add("Happy, Happy! Joy, joy!");
                 badList.Add("Hard work never killed anybody, but why take a chance?");
                 badList.Add("Haste maketh waste.");
                 badList.Add("Have faith, though it be only in a stone, and you will recover.");
                 badList.Add("Have you ever noticed that people who do things get most of their criticism from those who do nothing?");
                 badList.Add("Have you flogged your crew today?");
                 badList.Add("Having children is like having a bowling alley installed in your brain.");
                 badList.Add("He attacked me with a banana!");
                 badList.Add("He has the heart of a little child...  it's in a jar on his desk.");
                 badList.Add("He is a good story teller who can turn his ears into eyes.");
                 badList.Add("He is truly wise who gains wisdom from another's mishap.");
                 badList.Add("He slipped and fell on his own dagger in self-defense.");
                 badList.Add("He who dies with the most toys is still dead.");
                 badList.Add("He who drives an ass must of necessity know its wind.");
                 badList.Add("He who eats when he is full digs his grave with his teeth.");
                 badList.Add("He who has a shady past knows that nice guys finish last.");
                 badList.Add("He who has burned his mouth with milk blows on ice cream.");
                 badList.Add("He who has imagination without learning has wings but no feet.");
                 badList.Add("He who hesitates is sometimes saved.");
                 badList.Add("He who houses a camel must make his door higher.");
                 badList.Add("He who invents adages for others to peruse takes along rowboat when going on cruise.");
                 badList.Add("He who is a mocker dances without a tambourine.");
                 badList.Add("He who laughs last didn't get the joke.");
                 badList.Add("He who laughs, lasts.");
                 badList.Add("He who loses his head is usually the last one to miss it.");
                 badList.Add("He who reads many fortunes gets confused.");
                 badList.Add("He who sleeps in a marsh wakes up cousin to the frogs.");
                 badList.Add("He who slings mud loses ground.");
                 badList.Add("He who speaks the truth better have one foot in the stirrup.");
                 badList.Add("He who spends a storm beneath a tree, takes life with a grain of TNT.");
                 badList.Add("He who touches honey is compelled to lick his fingers.");

                 if (rankindex < 16 && rankindex > 0)
                 {
                     return goodList[rankindex];
                 }

                 rankindex =   rnd.Next(0, badList.Count);
                 return badList[rankindex];
        }

        private int getRankIndex(double newrank, SortedList<double, PlayerScore> TopScores)
        {
            int idx = 1;
            foreach (KeyValuePair<double,PlayerScore> item in TopScores.Reverse())
            {
                if (item.Key > newrank)
                {
                    idx++;
                }
            }
            return idx;
        }

        private void UpdateZOrder(UIElement element, bool bringToFront)
        {
            #region Safety Check

            if (element == null)
                throw new ArgumentNullException("element");

            if (!this.canvas1.Children.Contains(element))
                throw new ArgumentException("Must be a child element of the Canvas.", "element");

            #endregion // Safety Check

            #region Calculate Z-Indici And Offset

            // Determine the Z-Index for the target UIElement.
            int elementNewZIndex = -1;
            if (bringToFront)
            {
                foreach (UIElement elem in this.canvas1.Children)
                    if (elem.Visibility != Visibility.Collapsed)
                        ++elementNewZIndex;
            }
            else
            {
                elementNewZIndex = 0;
            }

            // Determine if the other UIElements' Z-Index 
            // should be raised or lowered by one. 
            int offset = (elementNewZIndex == 0) ? +1 : -1;

            int elementCurrentZIndex = Canvas.GetZIndex(element);

            #endregion // Calculate Z-Indici And Offset

            #region Update Z-Indici

            // Update the Z-Index of every UIElement in the Canvas.
            foreach (UIElement childElement in this.canvas1.Children)
            {
                if (childElement == element)
                    Canvas.SetZIndex(element, elementNewZIndex);
                else
                {
                    int zIndex = Canvas.GetZIndex(childElement);

                    // Only modify the z-index of an element if it is  
                    // in between the target element's old and new z-index.
                    if (bringToFront && elementCurrentZIndex < zIndex ||
                        !bringToFront && zIndex < elementCurrentZIndex)
                    {
                        Canvas.SetZIndex(childElement, zIndex + offset);
                    }
                }
            }

            #endregion // Update Z-Indici
        }


        private void canvas1_KeyDown(object sender, KeyEventArgs e)
        {
            if(tgb.GameOver == false)
            {
                switch (e.Key)
                {
                    case Key.D1: tgb.moveActivePiece(TetrisShape.TetrisMoves.ROTATELEFT);
                        break;
                    case Key.D2: tgb.moveActivePiece(TetrisShape.TetrisMoves.ROTATERIGHT);
                        break;              
                    case Key.Down: tgb.moveActivePiece(TetrisShape.TetrisMoves.DOWN);
                        break;               
                    case Key.Left: tgb.moveActivePiece(TetrisShape.TetrisMoves.LEFT);
                        break;                
                    case Key.Right: tgb.moveActivePiece(TetrisShape.TetrisMoves.RIGHT);
                        break;               
                    case Key.Up: tgb.moveActivePiece(TetrisShape.TetrisMoves.UP);
                        break;
                    case Key.Space: mnuPauseGame_Click(null, null);
                        break;
                    default:
                        break;
                }
            }
        }

        private void mnuNewGame_Click(object sender, RoutedEventArgs e)
        {
            if(webThread.ThreadState == System.Threading.ThreadState.Stopped)
            {

                webThread = new System.Threading.Thread(new System.Threading.ThreadStart(  FetchAndUpdateOnlineScores));
                webThread.Start();
            }
            startNewGame();
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            string lower = txtName.Text.ToLower();
            foreach (string  badWord in bw.BadWords)
            {
                if (lower.Contains(badWord))
                {
                    txtName.Text = lower.Replace(badWord, "");
                    break;
                }
            }

            if (e.Key == Key.Enter)
            {
                NewScore.Name = txtName.Text;
                Double rank = getRank(NewScore.Level, NewScore.Points);
                if (!TopScores.ContainsKey(rank))
                    TopScores.Add(rank, NewScore);

                Properties.Settings.Default.HighScores = serializeTopScores(this.TopScores);
                Properties.Settings.Default.Save();

                // we should upload our new score here.
                ossc.PutScore(NewScore.Name, NewScore.Level, NewScore.Points);
                
                displayHighScores();
                
                
                txtName.Visibility = Visibility.Hidden;
            }
        }

        private void mnuPauseGame_Click(object sender, RoutedEventArgs e)
        {
            if (tgb.GameOver == false)
            {
                if (tgb.gameRunning == true)
                {
                    tgb.Pause();
                    mnuPauseGame.InputGestureText = "Resume Game";
                }
                else
                {
                    tgb.Resume();
                    mnuPauseGame.InputGestureText = "Pause Game";
                }
            }
        }
    }
    public class TetrisAI
    {
        public TetrisAI(Wpf_tetris.engine.TetrisShape ActiveShape, Wpf_tetris.engine.TetrisShape SetPieces )
        {

        }
    }
}
