using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wpf_tetris
{
    class badwords
    {
        string allwords = "anus,butt,arse,butt,arsehole,butt,ass,butt,assbag,idiot,assbandit,homosexual,assbanger,homosexual,assbite,idiot,assclown,butt,asscock,idiot,asscracker,butt,asses,butts,assface,butt,assfuck,rear-loving,assfucker,homosexual,assgoblin,homosexual,asshat,butt,asshead,idiot,asshole,jerk,asshopper,homosexual,assjacker,homosexual,asslick,idiot,asslicker,Buttlicker,assmunch,idiot,assmuncher,butt,assshole,butt,asswipe,butt,bampot,idiot,bastard,illegitimate child,beaner,Mexican,bitch,female dog,bitchass,idiot,bitchtits,homosexual,bitchy,mean,blow job,sexual act,blowjob,sexual act,boner,erection,brotherfucker,homosexual,bullshit,poop,bumblefuck,homosexual,butt plug,cork,butt-pirate,homosexual,buttfucka,homosexual,buttfucker,homosexual,camel toe,female genitalia,carpetmuncher,homosexual,chinc,Chinese,chink,asian,choad,male genitalia,chode,small penis,clit,female genitals,clitfuck,sexual act,cock,penis,cockbite,idiot,cockface,idiot,cockfucker,idiot,cockknoker,homosexual,cockmaster,homosexual,cockmongruel,homosexual,cockmuncher,homosexual,cocksmoker,homosexual,cocksucker,homosexual,coochie,female genitalia,coochy,female genitalia,coon,African American,cooter,vagina,cracker,Caucasian,cum,semen,cumbubble,idiot,cumtart,idiot,cunnilingus,sexual act,cunt,vagina,cunthole,female genitalia,damn,darn,deggo,Italian,dick,penis,dickbag,idiot,dickbeaters,Hands,dickfuck,idiot,dickhead,phallace face,dickhole,male genitalia,dickmonger,homosexual,dicks,penises,dickweed,idiot,dickwod,idiot,dildo,sexual toy,dipshit,idiot,dookie,poop,douche,female hygene product,douche-fag,idiot,douchebag,female hygene accessory,douchewaffle,homosexual,dumass,idiot,dumb ass,idiot,dumbass,idiot,dumbfuck,idiot,dumbshit,idiot,dumshit,idiot,dyke,homosexual,fag,homosexual,fagbag,homosexual,fagfucker,homosexual,faggit,homosexual,faggot,homosexual,fagtard,homosexual idiot,fatass,a fat person,fellatio,sexual act,feltch,sexual act,fuck,fornicate,fuckass,idiot,fuckbrain,idiot,fucked,had intercourse,fucker,fornicator,fuckface,idiot,fuckhead,butt,fuckhole,jerk,fuckin,sexual act,fucking,freaking,fucknut,idiot,fucks,sexual act,fuckstick,male genitalia,fucktard,Moron,fuckup,idiot,fuckwad,idiot,fuckwit,dummy,fudgepacker,homosexual,gay,homosexual,gaybob,homosexual,gaydo,homosexual,gaytard,homosexual,gaywad,homosexual,goddamn,goshdarn,goddamnit,goshdarnit,gooch,female genitalia,gook,Chinese,gringo,foreigner,guido,italian,handjob,sexual act,hard on,erection,heeb,Jewish Person,hell,heck,woman,homo,homosexual,homodumbshit,idiot,honkey,white person,humping,sexual act,jackass,idiot,jap,japanesse person,jerk off,masturbate,jigaboo,African American,jizz,Semen,jungle bunny,african american,junglebunny,african american,kike,Jewish Person,kooch,female genitalia,kootch,female genitalia,kyke,Jewish person,lesbian,homosexual,lesbo,homosexual,lezzie,homosexual,mcfagget,homosexual,mick,irish,minge,female genitalia,mothafucka,Jerk,motherfucker,mother lover,motherfucking,fornicating with mother,muff,female genitalia,munging,sexual act,negro,african american,nigga,african american,nigger,african american,niglet,african american child,nut sack,male genitalia,nutsack,male genitalia,paki,pakistanien,panooch,femail genitalia,pecker,Penis,peckerhead,idiot,penis,male genitalia,penisfucker,homosexual,piss,urinate,pissed,urinated,pissed off,angry,pollock,polish person,poon,female genitals,poonani,female genitalia,poonany,vagina,porch monkey,african american,porchmonkey,African American,prick,penis,punta,female dog,pussies,Female Genitalias,pussy,female reproductive organ,pussylicking,sexual act,puto,idiot,queef,vaginal fart.,queer,homosexual,queerbait,homosexual,renob,erection,rimjob,dirty sexual act,sand nigger,middle eastern,sandnigger,middle eastern,schlong,male genitalia,scrote,male genitalia,shit,poop,shitbagger,idiot,shitcunt,idiot,shitdick,idiot,shitface,pooface,shitfaced,Drunk,shithead,jerk,shithouse,bathroom,shitspitter,butt,shitstain,poop,shitter,defecator,shittiest,worst,shitting,pooping,shitty,bad,skank,dirty girl,skeet,semen,skullfuck,sexual act,slut,sexually popular woman,slutbag,sexually popular woman,snatch,female genitalia,spic,mexican,spick,mexican american,splooge,ejaculate,tard,mentally challenged,testicle,male genitalia,thundercunt,idiot,tit,breast,titfuck,sexual act,tits,breasts,twat,female genitals,twatlips,idiot,twats,vaginas,twatwaffle,homosexual,va-j-j,female genitalia,vag,femail genitalia,vagina,female genitalia,vjayjay,female genitalia,wank,sexual act,wetback,mexican,whore,hussy,whorebag,idiot,wop,italian";
        List<string> words = new List<string>();
        public List<string> BadWords
        {
            get { return words; }
        }
        public badwords()
        {
            string[] ws = allwords.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in ws)
            {
                if (words.Contains(s) == false)
                    words.Add(s);
            }
        }
    }
}
