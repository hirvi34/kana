using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
// ReSharper disable All


namespace Kana;

/// @author Markus Honkanen
/// @version 29.11.2023
/// <summary>
/// funktio sekä taulukko url:https://tim.jyu.fi/view/kurssit/tie/ohj1/2023s/demot/demo11?answerNumber=7&amp;b=AsTtDdam5GeI&amp;size=1&amp;task=taulukot&amp;user=mvhonkzz#PhiIMpqFRlRA
/// 
/// </summary>
public class Kana : PhysicsGame
{

    

    private static String[] lines =
    {
        "  X                                                                                      ",
        "                                           X                                   X      B     ",
        "    X          X                                                   X             X         ",
        "                                                                                         ",
        "       X   B                       X                                                       ",
        "                                                                      X                    ",
        "                        X                                                                 ",
        "               X                                                       X              X     ",
        "                                            X                                 X            ",
        "             X         X                 X                                                   ",
        "               X                                            X                              ",
        "                        X                                                         X        ",
        "         X                                     X                        X                  ",
        "    X             X                                                                        ",
        "                                                                                  X      ",
        "                                  X                                                       ",
        "         X        X      X            X                                                      ",
        "                                                   X                                      ",
        "                                                                        X                 ",
        "   X    X                X              X                                                    ",
        "           X                                                       X                     X ",
        "       B                                      X                                            ",
        "                               X                           X                              ",
    };
    private static int tileWidth;
    private static int tileHeight;
    TileMap tiles;


    private Timer ajastin;
    private PhysicsObject Pelaaja1;
    
    // "eye candy"
    private Image i0 = LoadImage("box.png");
        
    // viholliset 1-3
    private Image i1 =LoadImage("vihu1.png");
    private Image i2 =LoadImage("vihu2.png");
    private Image i3 =LoadImage("vihu3.png");
        
    // pelaaja 4
    private Image i4 =LoadImage("kana.png");
        
    //taustat 5-8
    private Image i5 =LoadImage("tausta.png");
    private Image i6 =LoadImage("tausta1.png");
    private Image i7 =LoadImage("tausta2.png");
    private Image i8 =LoadImage("tausta3.png");
        
    // swat 9
    private Image i9 =LoadImage("swat.png");
    
    public override void Begin()
    {
        tileWidth = 1050 / lines[0].Length;
        tileHeight = 600 / lines.Length;
        tiles = TileMap.FromStringArray(lines);

        LuoTaso();
        Pelaaja(5 * 85, 10 * -30);
        
        tiles.SetTileMethod('X', Vihollinen);
        tiles.SetTileMethod('B', Laatikko);

        ajastin = new Timer();
        
        LuoOhjaimet();
        tiles.Execute(tileWidth, tileHeight);

        Valikot(0);
    }
    
    /// <summary>
    /// Voit valita mitä valikkoa haluut käyttää
    /// </summary>
    /// <param name="valikko">numero millä valitset valikon</param>
    private void Valikot(int valikko)
    {
        if (valikko == 0)
        {
            MultiSelectWindow alkuvalikko = new MultiSelectWindow("Menu", "Start", "End");
            alkuvalikko.AddItemHandler(0, () =>
            {
                ajastin.Start();
                aika();
            });
            alkuvalikko.AddItemHandler(1, Exit);
            
            Add(alkuvalikko);
        }

        if (valikko == 1)
        {
            MultiSelectWindow kuolema = new MultiSelectWindow("Loser",  "End");
            kuolema.AddItemHandler(0, Exit);
            
            Add(kuolema);
        }
    }

    /// <summary>
    /// kello joka tulee keskelle ruutua
    /// </summary>
    private void aika()
    {
        Label kello = new Label();
        kello.TextColor = Color.Black;
        kello.Position = new Vector(0, 0);
        kello.Text = "Time: 0";

        Timer paivita = new Timer();
        paivita.Interval = 0.01;
        paivita.Timeout += () =>
        {
            kello.Text = "Time: " + ajastin.CurrentTime.ToString("0.00");
        };
        paivita.Start();

        Add(kello);
    }
    
   /// <summary>
   /// Luo tason kaikki seinät
   /// </summary>
    private void LuoTaso()
    {
        PhysicsObject vasenReuna = Level.CreateLeftBorder();
        vasenReuna.Restitution = 1.5;
        

        PhysicsObject oikeaReuna = Level.CreateRightBorder();
        oikeaReuna.Restitution = 1.5;
        

        PhysicsObject keskitaso = new PhysicsObject(30 * 20.0, 2 * 20.0, Shape.Rectangle);
        keskitaso.Color = Color.Gray;
        keskitaso.Restitution = 1.5;
        keskitaso.Y = Level.Bottom + 20 * 20;
        keskitaso.X = oikeaReuna.X - 16 * 20;
        oikeaReuna.Add(keskitaso);

        PhysicsObject ylaReuna = Level.CreateTopBorder();
        ylaReuna.Restitution = 1.5;
        ylaReuna.Y = Level.Top;

        PhysicsObject alaReuna = Level.CreateBottomBorder();
        alaReuna.Restitution = 1.5;
        alaReuna.Y = Level.Bottom;

        PhysicsObject keskiseina = new PhysicsObject(2 * 20.0, 22 * 20.0, Shape.Rectangle);
        keskiseina.Color = Color.Gray;
        keskiseina.Restitution = 1.5;
        keskiseina.Y = Level.Bottom + 20 * 10.0;
        keskiseina.X = alaReuna.X - 20 * 10.0;
        alaReuna.Add(keskiseina);


        Level.Background.Image = i5;

        Camera.ZoomToLevel();
    }
    
   /// <summary>
   /// Pelaajan sijainti sekä ulkonäkö
   /// </summary>
   /// <param name="x">X koordinaatti </param>
   /// <param name="y">Y koordinaatti</param>
    private void Pelaaja(double x, double y)
    {
        Pelaaja1 = new PhysicsObject(30, 30, Shape.Rectangle);
        
        Pelaaja1.X = x;
        Pelaaja1.Y = y;
        Pelaaja1.Restitution = 1.0;
        Pelaaja1.KineticFriction = 0.0;
        
        AddCollisionHandler(Pelaaja1, "pahis", PelaajaOsu);
        AddCollisionHandler(Pelaaja1, "raha", PelaajaSai);
        Pelaaja1.Image = i4;
        Add(Pelaaja1);
    }

   /// <summary>
   /// Esteet kentällä
   /// </summary>
   /// <param name="paikka">Sijainti</param>
   /// <param name="leveys">Leveys</param>
   /// <param name="korkeus">Korkeus</param>
    private void Vihollinen(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject p1 = new PhysicsObject(leveys * 2, korkeus, Shape.Rectangle);
        
        p1.Position = paikka;
        p1.Tag = "pahis";
        p1.CanRotate = false;
        p1.Mass = Double.MaxValue;

        p1.CollisionIgnoreGroup = 1;

        p1.Image = RandomGen.SelectOne(i1, i2, i3);
        
        Add(p1);
    }

   /// <summary>
   /// Vihollinen joka seuraa pelaajaa
   /// </summary>
    private void Swat()
    {
        PhysicsObject p1 = new PhysicsObject(25, 25);
        p1.X = 10;
        p1.Y = 50;
        p1.Tag = "pahis";
        p1.CanRotate = false;
        p1.CollisionIgnoreGroup = 1;

        p1.Image = i9;
        
        FollowerBrain m = new FollowerBrain(Pelaaja1);
        m.Speed = 50;
        m.DistanceFar = 1000;
        p1.Brain = m;
        
        Add(p1);
    }
    
   /// <summary>
   /// Kerättävät laatikot kentällä
   /// </summary>
   /// <param name="paikka">paikka</param>
   /// <param name="leveys">leveys</param>
   /// <param name="korkeus">korkeus</param>
    private void Laatikko(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject laatikko = new PhysicsObject(leveys * 4, korkeus * 2, Shape.Rectangle);
        laatikko.Color = Color.DarkBrown;
        laatikko.Position = paikka;
        laatikko.Tag = "raha";
        
        laatikko.Image = i0;
        
        Add(laatikko);
    }

   /// <summary>
   /// kattoo onko kaikki laatikot kerätty
   /// </summary>
   /// <returns></returns>
    private bool Keratty()
    {
        foreach (PhysicsObject obj in GetObjectsWithTag("raha"))
        {
            if (obj.Tag.ToString() == "raha")
            {
                return false;
            }
        }
        
        return true;
    }

   /// <summary>
   /// kun laatikot on kerätty pysäyttää kellon
   /// </summary>
   /// <param name="time">aika</param>
    protected override void Update(Time time)
    {
        base.Update(time);

        if (Keratty())
        {
            ajastin.Stop();
            Pelaaja1.Destroy();

            return;
        }
    }
    
    /// <summary>
    /// Luodaan ohjaimet pelaajalle
    /// </summary>
    private void LuoOhjaimet()
    {
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, null, new Vector(-1000, 0));
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, null, new Vector(1000, 0));
        Keyboard.Listen(Key.Up, ButtonState.Down, LiikutaPelaajaa, null, new Vector(0, 1000));
        Keyboard.Listen(Key.Down, ButtonState.Down, LiikutaPelaajaa, null, new Vector(0, -1000));
    }

    /// <summary>
    /// Pelaajan kyky liikkua
    /// </summary>
    /// <param name="vektori">millä nopeudella pelaaja liikkuu</param>
    private void LiikutaPelaajaa(Vector vektori)
    {
        Pelaaja1.Push(vektori);
    }

    /// <summary>
    /// esteet voi tuhota pelaajan
    /// </summary>
    /// <param name="pelaaja">kentällä oleva pelaaja</param>
    /// <param name="kohde">kohde joka voi tuhota pelaajan</param>
    private void PelaajaOsu(PhysicsObject pelaaja, PhysicsObject kohde)
    {
        pelaaja.Destroy();
        Valikot(1);
    }
    
    /// <summary>
    /// asiat mitä pelaaja voi kerätä
    /// </summary>
    /// <param name="pelaaja">kentällä oleva pelaaja</param>
    /// <param name="kohde">kerättävä kohde</param>
    private void PelaajaSai(PhysicsObject pelaaja, PhysicsObject kohde)
    {
        kohde.Destroy();
        
        Swat();

        Level.Background.Image = RandomGen.SelectOne(i6, i7, i8);
    }
}