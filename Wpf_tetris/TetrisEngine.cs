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
namespace Wpf_tetris.engine
{

    public class PlayerScore
    {
        int _level, _points;
        string _name;

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        public int Points
        {
            get { return _points; }
            set { _points = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public PlayerScore(string serailizedScore)
        {
            string[] sarr = serailizedScore.Split('?');
            if (sarr.Length > 2)
            {
                _level = Convert.ToInt32(sarr[0]);
                _points = Convert.ToInt32(sarr[1]);
                _name = sarr[2];
            }
        }

        public string serialize()
        {
            return string.Format("{0}?{1}?{2};", _level, _points, _name);
        }
    }

    public interface TetrisData
    {
        Point localpoint { get; set; }
        Vector worldPointTranform { get; set; }
        Point afterTranform { get; }

        TetrisData CloneData();
    }
    public class TetrisDataOnly : TetrisData
    {
        Point _localpoint = new Point();
        Vector _worldPointTransform = new Vector();
        Point _afterTransform = new Point();
        #region TetrisData Members

        public Point localpoint
        {
            get { return _localpoint; }
            set
            {
                _localpoint = value;
                _afterTransform = Point.Add(_localpoint, _worldPointTransform);
            }
        }

        public Vector worldPointTranform
        {
            get { return _worldPointTransform; }
            set
            {
                _worldPointTransform = value;
                _afterTransform = Point.Add(_localpoint, _worldPointTransform);
            }
        }
        public Point afterTranform
        {
            get { return _afterTransform; }
        }
        public TetrisData CloneData()
        {
            Point p = new Point(localpoint.X, localpoint.Y);
            TetrisDataOnly tp = new TetrisDataOnly(p);
            tp.worldPointTranform = this.worldPointTranform;
            return tp;
        }
        #endregion
        public TetrisDataOnly(Point p)
        {
            localpoint = p;
            worldPointTranform = new Vector(0, 0);
        }
    }
    public class TetrisPoint : TetrisData
    {
        Point _localpoint = new Point();
        Vector _worldPointTransform = new Vector();
        Point _afterTransform = new Point();

        Shape _elipse = new Ellipse();
        Canvas _renderedOn = null;

        public TetrisData CloneData()
        {
            Point p = new Point(localpoint.X, localpoint.Y);
            TetrisDataOnly tp = new TetrisDataOnly(p);            
            tp.worldPointTranform = this.worldPointTranform;
            return tp;
        }

        public Canvas renderedOn
        {
            get { return _renderedOn; }
            set { _renderedOn = value; }
        }

        public Point localpoint
        {
            get { return _localpoint; }
            set
            {
                _localpoint = value;
                _afterTransform = Point.Add(_localpoint, _worldPointTransform);
            }
        }

        public Vector worldPointTranform
        {
            get { return _worldPointTransform; }
            set
            {
                _worldPointTransform = value;
                _afterTransform = Point.Add(_localpoint, _worldPointTransform);
            }
        }
        public Point afterTranform
        {
            get { return _afterTransform; }
        }

        public Shape shape
        {
            get { return _elipse; }
            set { _elipse = value; }
        }
        public TetrisPoint()
        {

        }
        public TetrisPoint(Point p, Shape e)
        {
            localpoint = p;
            worldPointTranform = new Vector(0, 0);
            _elipse = e;
        }
        public TetrisPoint(Point p)
        {
            localpoint = p;
            worldPointTranform = new Vector(0, 0);
        }

    }
    public interface TetrisShapeData
    {
        List<TetrisData> points { get; set; }
        
    }
    public class TetrisShapeDataOnly : TetrisShapeData
    {
        List<TetrisData> _points = new List<TetrisData>();
        public List<TetrisData> points
        {
            get { return _points; }
            set { _points = value; }
        }
        public bool collidesWith(TetrisShapeData otherShape)
        {
            return collidesWith(otherShape.points);
        }
        public bool collidesWith(List<TetrisData> otherPoints)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                for (int j = 0; j < otherPoints.Count; j++)
                {
                    if (_points[i].afterTranform == otherPoints[j].afterTranform)
                    {
                        // collision
                        return true;
                    }
                }
            }
            return false;
        }
        public TetrisShapeDataOnly projectMove(TetrisShape.TetrisMoves tm)
        {
            List<TetrisData> returnval = this.CloneData();
            TetrisShape.movePiece(returnval, tm);
            TetrisShapeDataOnly tsdo = new TetrisShapeDataOnly();
            tsdo.points = returnval;
            return tsdo;
        }
        public List<TetrisData> CloneData()
        {
            List<TetrisData> tmp = new List<TetrisData>();
            for (int i = 0; i < _points.Count; i++)
            {
                tmp.Add(_points[i].CloneData());
            }
            return tmp;
        }
    }
    public class TetrisShape : TetrisShapeData
    {
        public enum TetrisShapes
        {
            I, J, L, O, S, T, Z
        };
        public enum TetrisMoves
        {
            UP, DOWN, LEFT, RIGHT, ROTATELEFT, ROTATERIGHT
        };

        public static readonly Matrix _rotateLeft = new Matrix(0, -1, 1, 0, 0, 0);
        public static readonly Matrix _rotateRight = new Matrix(0, 1, -1, 0, 0, 0);
        public static readonly Matrix _moveLeft = new Matrix(1, 0, 0, 1, -1, 0);
        public static readonly Matrix _moveRight = new Matrix(1, 0, 0, 1, 1, 0);
        public static readonly Matrix _moveUp = new Matrix(1, 0, 0, 1, 0, -1);
        public static readonly Matrix _moveDown = new Matrix(1, 0, 0, 1, 0, 1);
        private static readonly List<Point> PointsForI = new List<Point>(new Point[] 
                { new Point(0f, 1f), new Point(0, 0), new Point(0, -1), new Point(0, -2) });
        private static readonly List<Point> PointsForJ = new List<Point>(new Point[] { new Point(0f, 1f), new Point(0, 0), new Point(0, -1), new Point(-1, -1) });
        private static readonly List<Point> PointsForL = new List<Point>(new Point[] { new Point(0f, 1f), new Point(0, 0), new Point(0, -1), new Point(1, -1) });
        private static readonly List<Point> PointsForO = new List<Point>(new Point[] { new Point(0f, 1f), new Point(0, 0), new Point(1, 0), new Point(1, 1) });
        private static readonly List<Point> PointsForS = new List<Point>(new Point[] { new Point(0f, 1f), new Point(0, 0), new Point(1, 0), new Point(1, -1) });
        private static readonly List<Point> PointsForT = new List<Point>(new Point[] { new Point(0f, 1f), new Point(0, 0), new Point(0, -1), new Point(1, 0) });
        private static readonly List<Point> PointsForZ = new List<Point>(new Point[] { new Point(0f, 1f), new Point(0, 0), new Point(-1, 0), new Point(-1, -1) });

        List<TetrisPoint> _points = new List<TetrisPoint>();

        // private ObservableCollection<Point> _points = new ObservableCollection<Point>();
        public List<TetrisPoint> points
        {
            get { return _points ; }
            set { _points = value; }
        }

        private Color _color = new Color();
        public Color color
        {
            get { return _color;  }
            set { _color = value; }
        }

        private int _shapeHeight = 0;
        public int ShapeHeight
        {
            get { return _shapeHeight; }
            set { _shapeHeight = value; }
        }
        private bool _activated = false;
        public bool Activated
        {
            get { return _activated; }
            set { _activated = value; }
        }

        public List<TetrisData> CloneData()
        {
            List<TetrisData> tmp = new List<TetrisData>();
            for (int i = 0; i < _points.Count; i++)
            {
                tmp.Add(_points[i].CloneData());
            }            
            return tmp;
        }

        public TetrisShape(List<TetrisPoint> points)
        {
            _points.Clear();
            _points = points;
            _color = Colors.Silver;
        }
        
        public TetrisShape(TetrisShapes shape)
        {
            List<Point> SomePoints = new List<Point>();
            switch (shape)
            {
                case TetrisShapes.I: SomePoints = (PointsForI); color = Colors.Red; ShapeHeight = 4;
                    break;
                case TetrisShapes.J: SomePoints = (PointsForJ); color = Colors.White; ShapeHeight = 3;
                    break;
                case TetrisShapes.L: SomePoints = (PointsForL); color = Colors.Magenta; ShapeHeight = 3;
                    break;
                case TetrisShapes.O: SomePoints = (PointsForO); color = Colors.Blue; ShapeHeight = 2;
                    break;
                case TetrisShapes.S: SomePoints = (PointsForS); color = Colors.Green; ShapeHeight = 3;
                    break;
                case TetrisShapes.T: SomePoints = (PointsForT); color = Colors.Brown; ShapeHeight = 3;
                    break;
                case TetrisShapes.Z: SomePoints = (PointsForZ); color = Colors.Cyan; ShapeHeight = 3;
                    break;
                default: SomePoints = (PointsForI); color = Colors.Red;
                    break;
            }
            foreach (Point p in SomePoints)
            {
                System.Windows.Shapes.Rectangle e = new Rectangle();
                e.Width = 16;
                e.Height = 16;
                e.Fill = new SolidColorBrush(color);
                e.SetValue(Canvas.LeftProperty, p.X);
                e.SetValue(Canvas.TopProperty, p.Y);
                _points.Add(new TetrisPoint(p, e));
            }
        }
        public void setStroke(Brush stroke)
        {
            foreach (var item in this._points)
            {
                (item as TetrisPoint).shape.Stroke = stroke;
                //item.shape.Stroke = stroke;
            }
        }
        private void transformPoints(Matrix m)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                _points[i].localpoint = Point.Multiply(_points[i].localpoint, m);
            }
        }
        private static void transformPoints(List<TetrisData> _points, Matrix m)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                _points[i].localpoint = Point.Multiply(_points[i].localpoint, m);
            }
        }
        private void transformPoints(Vector v)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                _points[i].worldPointTranform += v;
            }
        }
        private static void transformPoints(List<TetrisData> _points, Vector v)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                _points[i].worldPointTranform += v;
            }
        }

        public bool collidesWith(TetrisShapeData otherShape)
        {
            return collidesWith(otherShape.points);
        }
        public bool collidesWith(List<TetrisData> otherPoints)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                for (int j = 0; j < otherPoints.Count; j++)
                {
                    if (_points[i].afterTranform == otherPoints[j].afterTranform)
                    {
                        // collision
                        return true;
                    }
                }
            }
            return false;
        }
        public void clearPoint(Point p)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                if (_points[i].afterTranform.X == p.X &&
                    _points[i].afterTranform.Y == p.Y)
                {
                    _points.RemoveAt(i);
                    return;
                }
            }
        }
        private static void rotateRight(List<TetrisData> points, int count)
        {
            for (int i = 0; i < count; i++)
            {
                transformPoints(points,_rotateRight);
            }
        }
        private void rotateRight(int count)
        {
            for (int i = 0; i < count; i++)
            {
                transformPoints(_rotateRight);
            }
        }
        private static void rotateLeft(List<TetrisData> points, int count)
        {
            for (int i = 0; i < count; i++)
            {
                transformPoints(points,_rotateLeft);
            }
        }
        private void rotateLeft(int count)
        {
            for (int i = 0; i < count; i++)
            {
                transformPoints(_rotateLeft);
            }
        }
        private static void moveLeft(List<TetrisData> points, int count)
        {
            transformPoints(points, new Vector(-count, 0));
        }
        private void moveLeft(int count)
        {
            transformPoints(new Vector(-count, 0));
        }
        private static void moveRight(List<TetrisData> points, int count)
        {
            transformPoints(points, new Vector(count, 0));
        }
        private void moveRight(int count)
        {
            transformPoints(new Vector(count, 0));
        }
        private static void moveUp(List<TetrisData> points, int count)
        {
            transformPoints(points, new Vector(0, -count));
        }
        private void moveUp(int count)
        {
            transformPoints(new Vector(0, -count));
        }
        private static void moveDown(List<TetrisData> points, int count)
        {
            transformPoints(points, new Vector(0, count));
        }
        private void moveDown(int count)
        {
            transformPoints(new Vector(0, count));
        }


        public TetrisShapeDataOnly projectMove(TetrisMoves tm)
        {
            List<TetrisData> returnval = this.CloneData();
            movePiece(returnval,tm);
            TetrisShapeDataOnly tsdo = new TetrisShapeDataOnly();
            tsdo.points = returnval;
            return tsdo;
        }
        public static void movePiece(List<TetrisData> points, TetrisMoves tm)
        {
            movePiece(points, tm, 1);
        }
        public static void movePiece(List<TetrisData> points, TetrisMoves tm, int count)
        {
            switch (tm)
            {
                case TetrisMoves.UP: moveUp(points, count);
                    break;
                case TetrisMoves.DOWN: moveDown(points, count);
                    break;
                case TetrisMoves.LEFT: moveLeft(points, count);
                    break;
                case TetrisMoves.RIGHT: moveRight(points, count);
                    break;
                case TetrisMoves.ROTATELEFT: rotateLeft(points, count);
                    break;
                case TetrisMoves.ROTATERIGHT: rotateRight(points, count);
                    break;
                default:
                    break;
            }
        }
        public void movePiece(TetrisMoves tm)
        {
            movePiece(tm, 1);
        }
        public void movePiece(TetrisMoves tm, int count)
        {
            switch (tm)
            {
                case TetrisMoves.UP: moveUp(count);
                    break;
                case TetrisMoves.DOWN: moveDown(count);
                    break;
                case TetrisMoves.LEFT: moveLeft(count);
                    break;
                case TetrisMoves.RIGHT: moveRight(count);
                    break;
                case TetrisMoves.ROTATELEFT: rotateLeft(count);
                    break;
                case TetrisMoves.ROTATERIGHT: rotateRight(count);
                    break;
                default:
                    break;
            }
        }

        #region TetrisShapeData Members

        List<TetrisData> TetrisShapeData.points
        {
            get
            {
                List<TetrisData> tmp = new List<TetrisData>();
                for (int i = 0; i < _points.Count; i++)
                {
                    tmp.Add(_points[i]);
                }
                return tmp;
            }
            set
            {

            }
        }

        #endregion
    }

    public class ScoreChangedEventArgs
    {
        public int Score;
        public int Lines;
        public int TotoalLines;
        public int Level;
        public bool gameOver;

        public ScoreChangedEventArgs()
        {

        }
    }

    public class TetrisGameBoard : List<TetrisShape>
    {
        public delegate void ScoreChangedEventHandler(ScoreChangedEventArgs args);
        public event ScoreChangedEventHandler ScoreChangedEvent;

        bool _gameOver = false;
        public bool GameOver
        {
            get { return _gameOver; }
            set
            {
                _gameOver = value;
                if (_gameOver == true)
                {
                    _gameTimer.Stop();
                    RaiseScoreChangedEvent();
                    _activeShape = new TetrisShape(TetrisShape.TetrisShapes.O);
                }
            }
        }

        // PointCollection _boardBoarders = new PointCollection();
        TetrisShape _boardBoarder;
        TetrisShape _boardBackground;
        // List<TetrisPoint> _boardBoarders = new List<TetrisPoint>();
        TetrisShape _activeShape;
        System.Collections.Generic.Queue<TetrisShape> shapeQueue;
        Canvas boardCanvas;
        Random _rnd;
        System.Timers.Timer _gameTimer = new Timer(400);
        Action TimerAction = null;
        public TetrisShape BoardBoarder
        {
            get { return _boardBoarder; }
            set { _boardBoarder = value; }
        }
        public bool gameRunning
        {
            get { return _gameTimer.Enabled; }
        }
        int _linesCleared = 0;
        int _linesClearedThisLevel = 0;
        int _score = 0;
        int _level = 1;
        Brush Border1 = null;
        int nameTracker = 0;
        public TetrisGameBoard(Canvas drawingCanvas, Random rnd, Brush brush1)
        {
            init(drawingCanvas, rnd, brush1);
        }

        public TetrisGameBoard(Canvas drawingCanvas, Random rnd)
        {
            init(drawingCanvas, rnd, null);
        }

        private void init(Canvas drawingCanvas, Random rnd, Brush brush1)
        {
            Border1 = brush1;
            _rnd = rnd;
            // fill up the queue, then pop us an active shape.
            shapeQueue = new Queue<TetrisShape>();
            boardCanvas = drawingCanvas;

            NameScope.SetNameScope(boardCanvas, new NameScope());

            InitializeBoardBoarderStandard();
            InitializeBoardBackgroundStandard();

            _boardBoarder.setStroke(Border1);// set the stroke around the pieces.

            updateShape(_boardBoarder, TimeSpan.FromMilliseconds(500));// the boarder
            updateShape(_boardBackground, TimeSpan.FromMilliseconds(500));// the background.

            TimerAction += new Action(TimerActionDelegate);
            _gameTimer.Elapsed += new ElapsedEventHandler(gameTimer_Elapsed);

            _gameTimer.Start();
        }


        public void Pause()
        {
            this._gameTimer.Enabled = false;
        }
        public void Resume()
        {
            this._gameTimer.Enabled = true;
        }

        void TimerActionDelegate()
        {
            while (shapeQueue.Count < 5)
                queueNewPiece();

            if (_activeShape == null)
            {
                loadActiveShapeFromQueue();
                updateShape(_activeShape, TimeSpan.FromMilliseconds(_gameTimer.Interval)); // active piece
            }

            if (_activeShape.Activated == true)
            {
                moveActivePiece(TetrisShape.TetrisMoves.DOWN);
            }
            else
            {
                _activeShape.Activated = true;
            }


        }

        void gameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_boardBoarder.points.Count > 0)
                (_boardBoarder.points[0] as TetrisPoint).shape.Dispatcher.BeginInvoke(DispatcherPriority.Normal, TimerAction);
        }
        private void InitializeBoardBoarderStandard()
        {
            Point[] leftSide = new Point[] { 
                new Point(-5f, -9f), 
                new Point(-5f, -8f), 
                new Point(-5f, -7f), 
                new Point(-5f, -6f), 
                new Point(-5f, -5f),
                new Point(-5f, -4f), 
                new Point(-5f, -3f), 
                new Point(-5f, -2f),
                new Point(-5f, -1f),
                new Point(-5f, 9f), 
                new Point(-5f, 8f), 
                new Point(-5f, 7f), 
                new Point(-5f, 6f), 
                new Point(-5f, 5f),
                new Point(-5f, 4f), 
                new Point(-5f, 3f), 
                new Point(-5f, 2f),
                new Point(-5f, 1f), 
                new Point(-5f, 0f), 
                 };
            Point[] rightSide = new Point[] { 
                new Point(5f, -9f), 
                new Point(5f, -8f), 
                new Point(5f, -7f), 
                new Point(5f, -6f), 
                new Point(5f, -5f),
                new Point(5f, -4f), 
                new Point(5f, -3f), 
                new Point(5f, -2f),
                new Point(5f, -1f),
                new Point(5f, 9f), 
                new Point(5f, 8f), 
                new Point(5f, 7f), 
                new Point(5f, 6f), 
                new Point(5f, 5f),
                new Point(5f, 4f), 
                new Point(5f, 3f), 
                new Point(5f, 2f),
                new Point(5f, 1f),
                new Point(5f, 0f)
                 };
            Point[] bottomSide = new Point[] { 
                new Point(5f, 10f), 
                new Point(4f, 10f), 
                new Point(3f, 10f), 
                new Point(2f, 10f), 
                new Point(1f, 10f), 
                new Point(0f, 10f),
                new Point(-1f, 10f), 
                new Point(-2f, 10f), 
                new Point(-3f, 10f), 
                new Point(-4f, 10f),
                new Point(-5f, 10f)};
            Point[] topSide = new Point[] { 
                new Point(5f, -10f), 
                new Point(5f, -10f), 
                new Point(5f, -11f), 
               new Point(5f, -12f), 
                new Point(5f, -13f), 
                new Point(5f, -14f),
                new Point(5f, -15f),
                new Point(5f, -16f),
                new Point(5f, -17f),
                //new Point(0f, -10f),
                new Point(5f, -17f),
                new Point(5f, -16f),
                new Point(-5, -15f),
                new Point(-5, -14),
                new Point(-5f, -13f), 
               new Point(-5f, -12f), 
                new Point(-5f, -11f), 
                new Point(-5f, -10f),
                new Point(-5f, -10f)};
            IEnumerable<Point> ps = topSide.Concat<Point>(bottomSide).Concat<Point>(leftSide).Concat<Point>(rightSide);
            List<TetrisPoint> tl = new List<TetrisPoint>();
            foreach (Point p in ps)
            {
                System.Windows.Shapes.Rectangle e = new Rectangle();
                e.Width = 16;
                e.Height = 16;
                e.Fill = Brushes.DarkSlateGray;
                e.SetValue(Canvas.LeftProperty, p.X);
                e.SetValue(Canvas.TopProperty, p.Y);
                TetrisPoint tp = new TetrisPoint(p, e);
                tl.Add(tp);
            }
            TetrisShape ts = new TetrisShape(tl);
            _boardBoarder = ts;
        }
        private void InitializeBoardBackgroundStandard()
        {
            Color c = new Color();
            c.R = 15;
            c.G = 15;
            c.B = 15;
            c.A = 255;
            SolidColorBrush s = new SolidColorBrush(c);

            // The top gray Dark gray section
            IEnumerable<Point> ps = null;
            for (int i = 0; i < 19; i++)
            {
                double j = 9f - i;
                Point[] topSide = new Point[] { 
                new Point(4f, j), 
                new Point(3f, j), 
                new Point(2f, j), 
                new Point(1f, j), 
                new Point(0f, j),
                new Point(-1, j),
                new Point(-2, j),
                new Point(-3f, j), 
                new Point(-4f, j)};

                if (ps == null)
                {
                    ps = topSide.ToList();
                }
                else
                {
                    ps = ps.Concat(topSide.ToList());
                }
            }

            List<TetrisPoint> tl = new List<TetrisPoint>();
            foreach (Point p in ps)
            {
                System.Windows.Shapes.Rectangle e = new Rectangle();
                e.Width = 16;
                e.Height = 16;

                e.Fill = s;
                e.SetValue(Canvas.LeftProperty, p.X);
                e.SetValue(Canvas.TopProperty, p.Y);
                TetrisPoint tp = new TetrisPoint(p, e);
                tl.Add(tp);
            }
            TetrisShape ts = new TetrisShape(tl);
            _boardBackground = ts;
        }

        private void RaiseScoreChangedEvent()
        {
            ScoreChangedEventArgs score = new ScoreChangedEventArgs();
            score.Level = this._level;
            score.Lines = this._linesClearedThisLevel;
            score.TotoalLines = this._linesCleared;
            score.Score = this._score;
            score.gameOver = this.GameOver;

            if (ScoreChangedEvent != null)
            {
                ScoreChangedEvent(score);
            }
        }

        private void queueNewPiece()
        {
            TetrisShape.TetrisShapes ts1 = (TetrisShape.TetrisShapes)Convert.ToInt32(_rnd.NextDouble() * 7);
            TetrisShape.TetrisShapes ts2 = (TetrisShape.TetrisShapes)Convert.ToInt32(_rnd.NextDouble() * 7);
            TetrisShape newShape;
            if (ts1 == TetrisShape.TetrisShapes.I)
            {
                newShape = new TetrisShape(ts1);
            }
            else
            {
                newShape = new TetrisShape(ts2);
            }
            // set the style
            newShape.setStroke(Border1);


            newShape.movePiece(TetrisShape.TetrisMoves.LEFT, 8);
            newShape.movePiece(TetrisShape.TetrisMoves.DOWN, 8);
            shapeQueue.Enqueue(newShape);
            // move all piece is the queue up enough space to fit the new piece.
            foreach (TetrisShape ts in shapeQueue)
            {
                ts.movePiece(TetrisShape.TetrisMoves.UP, 4);
                updateShape(ts, TimeSpan.FromMilliseconds(_gameTimer.Interval));
            }
        }

        private void loadActiveShapeFromQueue()
        {
            if (shapeQueue.Count > 0)
            {
                _activeShape = shapeQueue.Dequeue();
                queueNewPiece();
                _activeShape.movePiece(TetrisShape.TetrisMoves.RIGHT, 8);
                _activeShape.movePiece(TetrisShape.TetrisMoves.UP, 2);
            }
        }

        private void updateShape(List<TetrisShape> lts, Duration d)
        {
            foreach (TetrisShape ts in lts)
            {
                updateShape(ts, d);
            }
        }

        private void updateShape(TetrisShape ts, Duration d)
        {
            double canvasWidthByTwo = boardCanvas.ActualWidth / 2;
            double canvasHeightByTwo = boardCanvas.ActualHeight / 2;
            double canvasHeightBy30 = boardCanvas.ActualHeight / 30;
            DoubleAnimation dax, day;
            Storyboard sb = new Storyboard();
            foreach (TetrisPoint tp in ts.points)
            {
                dax = new DoubleAnimation(canvasWidthByTwo + (tp.afterTranform.X * canvasHeightBy30), d, FillBehavior.HoldEnd);
                day = new DoubleAnimation(canvasHeightByTwo + (tp.afterTranform.Y * canvasHeightBy30), d, FillBehavior.HoldEnd);
                if (tp.renderedOn != boardCanvas)
                {
                    tp.shape.Name = "tempName" + (++nameTracker).ToString();
                    boardCanvas.RegisterName(tp.shape.Name, tp.shape);

                    boardCanvas.Children.Add(tp.shape);
                    tp.renderedOn = boardCanvas;
                }


                
                sb.Children.Add(dax);
                sb.Children.Add(day);
                Storyboard.SetTargetName(dax, tp.shape.Name);
                Storyboard.SetTargetName(day, tp.shape.Name);
                Storyboard.SetTargetProperty(dax, new PropertyPath(Canvas.LeftProperty));
                Storyboard.SetTargetProperty(day, new PropertyPath(Canvas.TopProperty));

                


                // tp.shape.BeginAnimation(Canvas.LeftProperty, dax,HandoffBehavior.SnapshotAndReplace);
                // tp.shape.BeginAnimation(Canvas.TopProperty, day,HandoffBehavior.SnapshotAndReplace);
            }
            sb.Begin(this.boardCanvas, false);
        }


        public void moveActivePiece(TetrisShape.TetrisMoves tm)
        {
            moveActivePiece(tm, true);
        }

        public void moveActivePiece(TetrisShape.TetrisMoves tm, bool resetTimer)
        {

            if (_activeShape == null || _activeShape.Activated == false || _gameTimer.Enabled == false)
                return;

            // Up is a special command, we want to drop down to the bottom.
            int howFarToDrop = 1;
            bool dropToBottom = (tm == TetrisShape.TetrisMoves.UP);
            if (dropToBottom)
                tm = TetrisShape.TetrisMoves.DOWN;

            if (resetTimer && tm == TetrisShape.TetrisMoves.DOWN)
            {
                _gameTimer.Enabled = false;
            }

            bool canMove = true;
            TetrisShapeDataOnly ts = _activeShape.projectMove(tm); ;

            canMove = isInValidPosition(ts);

            // Two special cases to check for.
            if (canMove == false && (tm == TetrisShape.TetrisMoves.ROTATELEFT || tm == TetrisShape.TetrisMoves.ROTATERIGHT))
            {
                // lets see if sliding the piece left or right would let us move.. If it does, then lets do it.
                // this will allow us to rotate when we are up against the wall, or trying to rotate into a tight spot.
                TetrisShapeDataOnly tsLeft = ts.projectMove(TetrisShape.TetrisMoves.LEFT);//.projectMove(TetrisShape.TetrisMoves.LEFT);
                if (isInValidPosition(tsLeft))
                {
                    // lets apply this left move right here.
                    _activeShape.movePiece(TetrisShape.TetrisMoves.LEFT);
                    canMove = true;
                }
                else
                {
                    TetrisShapeDataOnly tsRight = ts.projectMove(TetrisShape.TetrisMoves.RIGHT);
                    if (isInValidPosition(tsRight))
                    {
                        // lets apply this right move right here.
                        _activeShape.movePiece(TetrisShape.TetrisMoves.RIGHT);
                        canMove = true;
                    }
                }
            }


            if (canMove == false)
            {
                // find out if it collided while moving down
                if (tm == TetrisShape.TetrisMoves.DOWN)
                {
                    // if it i then move it into the local piece collection                     
                    this.Add(_activeShape);
                    // find any full rows and destory them.
                    int rowsCleared = this.ClearFullRows();

                    if (rowsCleared > 0)
                    {
                        // Update our score and Level
                        this._linesClearedThisLevel += rowsCleared;
                        this._linesCleared += rowsCleared;
                        this._score += Convert.ToInt32((rowsCleared * rowsCleared) * (10 - (_gameTimer.Interval / 100)));

                        if (this._linesClearedThisLevel >= 10 + this._level)
                        {
                            LevelChange();
                        }
                        this.RaiseScoreChangedEvent();
                    }
                    // and start a new active piece.
                    loadActiveShapeFromQueue();
                    updateShape(_activeShape, TimeSpan.FromMilliseconds(this._gameTimer.Interval));
                }
            }

            if (canMove && dropToBottom == false)
            {
                _activeShape.movePiece(tm);
                updateShape(_activeShape, TimeSpan.FromMilliseconds(100));
            }
            else if (canMove && dropToBottom == true)// if we are droping to bottom, find out how far we have to go to do that.    
            {
                // we wanna go until we cant move.

                while (canMove == true)
                {
                    ts = ts.projectMove(tm); // keep projecting down.

                    canMove = isInValidPosition(ts);

                    howFarToDrop++;
                }
                _activeShape.movePiece(TetrisShape.TetrisMoves.DOWN, --howFarToDrop);
                updateShape(_activeShape, TimeSpan.FromMilliseconds(100));
            }
            if (resetTimer && tm == TetrisShape.TetrisMoves.DOWN && this.GameOver == false)
            {
                _gameTimer.Enabled = !_gameTimer.Enabled;
            }
        }

        private void LevelChange()
        {
            this._level += 1;
            this._linesClearedThisLevel = 0;
            _gameTimer.Interval *= .90;

            LinearGradientBrush lgb  = (LinearGradientBrush) Border1;

            lgb.StartPoint = new Point(_rnd.NextDouble(), _rnd.NextDouble());
            lgb.EndPoint = new Point(_rnd.NextDouble(), _rnd.NextDouble());
            
        }

        public bool isInValidPosition(TetrisShapeDataOnly ts)
        {
            bool isValid = true;
            if (!ts.collidesWith(_boardBoarder))
            {
                // check against already set pieces.
                bool collidedwithaPiece = false;

                for (int i = 0; i < this.Count; i++)
                {
                    if (ts.collidesWith(this[i])) // shouldn't project on every one.
                    {
                        collidedwithaPiece = true;
                        break;
                    }
                }
                isValid = !collidedwithaPiece;
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }


        private int ClearFullRows()
        {
            int rowsCleared = 0;
            Vector down = new Vector(0, 1);
            Dictionary<int, int> pointsOnARow = new Dictionary<int, int>();

            for (int i = -30; i < 30; i++)
            {
                pointsOnARow.Add(i, 0);
            }

            foreach (TetrisShape piece in this)
            {
                foreach (TetrisPoint tp in piece.points)
                {
                    int idx = Convert.ToInt32(tp.worldPointTranform.Y + tp.localpoint.Y);
                    pointsOnARow[idx] += 1;
                }
            }

            List<TetrisPoint> pointsToKill = new List<TetrisPoint>();
            pointsToKill.Clear();
            Storyboard sb = new Storyboard();
            foreach (KeyValuePair<int, int> rowCount in pointsOnARow)
            {
                if (rowCount.Value > 8)
                {
                    rowsCleared++;
                    // Full row.. we need to explode this and destory these points.

                    for (int i = 0; i < this.Count; i++)// for each piece
                    {

                        foreach (TetrisPoint tp in this[i].points)// for each point on the piece.
                        {
                            if (Convert.ToInt32(tp.worldPointTranform.Y + tp.localpoint.Y) == rowCount.Key)
                            {
                                // destroy this point.
                                pointsToKill.Add(tp);

                                // start some animations for these, then we want to acutally remove them later.                        
                                Shape s = tp.shape;
                                // pointsToKill[j].shape.Fill = Brushes.Wheat;

                                DoubleAnimation day = new DoubleAnimation(2, TimeSpan.FromMilliseconds(800));
                                DoubleAnimation dax = new DoubleAnimation(2, TimeSpan.FromMilliseconds(800));
                                DoubleAnimation dal = new DoubleAnimation(-30, TimeSpan.FromMilliseconds(800));
                                DoubleAnimation dat = new DoubleAnimation(-30, TimeSpan.FromMilliseconds(800));
                                sb.Children.Add(day);
                                sb.Children.Add(dax);
                                sb.Children.Add(dal);
                                sb.Children.Add(dat);
                                Storyboard.SetTargetName(day, s.Name);
                                Storyboard.SetTargetName(dax, s.Name);
                                Storyboard.SetTargetName(dal, s.Name);
                                Storyboard.SetTargetName(dat, s.Name);
                                Storyboard.SetTargetProperty(day, new PropertyPath(Shape.HeightProperty));
                                Storyboard.SetTargetProperty(dax, new PropertyPath(Shape.WidthProperty));
                                Storyboard.SetTargetProperty(dal, new PropertyPath(Canvas.LeftProperty));
                                Storyboard.SetTargetProperty(dat, new PropertyPath(Canvas.TopProperty));
                                System.Windows.Forms.Application.DoEvents();
                            }
                        }



                    }
                }
            }
            sb.Begin(this.boardCanvas);

            System.Windows.Forms.Application.DoEvents();
            TetrisPoint tmp = null;
            while (pointsToKill.Count > 0)// this loop pauses us until our animations finish.
            {
                System.Windows.Forms.Application.DoEvents();
                tmp = pointsToKill.First();
                if ((double)tmp.shape.GetValue(Canvas.TopProperty) <= 0)
                {
                    boardCanvas.Children.Remove(tmp.shape);
                    var vv = from lots in this
                             where lots.points.Contains(tmp)
                             select lots;
                    vv.First().points.Remove(tmp);
                    pointsToKill.Remove(tmp);
                    tmp.shape = null;
                    tmp = null;
                }
                System.Windows.Forms.Application.DoEvents();
                System.Windows.Forms.Application.DoEvents();
            }
            pointsToKill.Clear();
            // at this stage all the points have been removed we just need to shift the board down.

            foreach (KeyValuePair<int, int> row in pointsOnARow)
            {
                if (row.Value > 8)
                {
                    // so every point above this key should drop one row.
                    for (int i = 0; i < this.Count; i++)
                    {
                        foreach (TetrisPoint tp in this[i].points)
                        {
                            if (Convert.ToInt32(tp.worldPointTranform.Y + tp.localpoint.Y) <= row.Key)
                            {
                                tp.worldPointTranform += down;
                                //tp.localpoint.Offset(0,1);//  += 1;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < this.Count; i++)
            {
                updateShape(this[i], TimeSpan.FromMilliseconds(30));
            }


            foreach (KeyValuePair<int, int> rowCount in pointsOnARow)
            {
                if (rowCount.Value > 0 && rowCount.Key <= -10)
                {
                    this.GameOver = true;
                }
            }



            return rowsCleared;
        }
    } 
}
