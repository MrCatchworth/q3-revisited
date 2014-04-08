using System;
using SFML.Graphics;
using SFML.Audio;
using SFML.Window;
using System.Diagnostics;

namespace JohnnySpaceGame {
    public class EntryPoint {
        public static void OnClosed(object sender, EventArgs e) {
            RenderWindow w = (RenderWindow)sender;
            w.Close();
        }
        public static void Main(string[] args) {
            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Fluffy Camelid Simulator 2014", Styles.Close);
            window.Closed += new EventHandler(OnClosed);
            const int fpsLimit = 60;
            window.SetFramerateLimit(fpsLimit);
            
            View viewingPlayer = new View(new FloatRect(0f,0f,800f,600f));
            viewingPlayer.Zoom(1.4f);
            
            ShipClass coolClass = new ShipClass(
                "Best Ship Ever",
                
                375f,
                2.0f,
                70f,
                
                150f,
                6.4f,
                120f,
                
                100,
                
                new WeaponSlot.Template[] {
                    new WeaponSlot.Template(new Vector2f(14f, -6f), 1, 5),
                    new WeaponSlot.Template(new Vector2f(14f, 6f), 1, 5)
                },
                new Texture("coolship.png"),
                27.5f,
                
                new EngineFlareData[] {
                    new EngineFlareData(new Vector2f(-18f, 0f), 9f)
                },
                new Color(0,255,255)
            );
            
            ShipClass arcliteTest = new ShipClass(
                "Arclite",
                
                600f,
                4.1f,
                75f,
                
                180f,
                12.8f,
                300f,
                
                70,
                
                new WeaponSlot.Template[] {
                    new WeaponSlot.Template(new Vector2f(-2f,-3f), 1, 4),
                    new WeaponSlot.Template(new Vector2f(-2f, 3f), 1, 4)
                },
                new Texture("arclite.png"),
                20f,
                
                new EngineFlareData[] {
                    new EngineFlareData(new Vector2f(-17f, -3f), 4f),
                    new EngineFlareData(new Vector2f(-17f, 3f), 4f)
                },
                new Color(0,255,255)
            );
            
            //Random bob = new Random();
            
            Faction dorf = new Faction("Dwarven Federal Armed Forces");
            Faction elf = new Faction("Elven Security Society");
            
            dorf.SetRelation(elf, -1);
            elf.SetRelation(dorf, -1);
            
            Encounter coolFight = new Encounter();
            
            for (int i=0; i<20; i++) {
                new Ship(new Vector2f(10f, i*40f), 0f, arcliteTest, new FightRandomAI(), dorf).Location = coolFight;
                new Ship(new Vector2f(790f, i*40f), Util.fPi, coolClass, new FightRandomAI(), elf).Location = coolFight;
            }
            
            
            Ship plyShip = new Ship(new Vector2f(400f,300f), 0f, arcliteTest, new KeyboardAI(), new Faction("Neutral Dummy"));
            plyShip.Location = coolFight;
            
            //new Ship(new Vector2f(400f,300f), 0f, arcliteTest, new PointAtAI(plyShip), dorf).Location = coolFight;
            
            Text memUsageText = new Text("", new Font("arial.ttf"));
            memUsageText.Color = new Color(255,255,255,255);
            memUsageText.Position = new Vector2f(10f,10f);
            
            while (window.IsOpen()) {
                window.DispatchEvents();
                window.Clear();
                
                coolFight.UpdateAll(1f/(float)fpsLimit);
                
                viewingPlayer.Center = plyShip.Pos;
                window.SetView(viewingPlayer);
                
                coolFight.DrawAll(window);
                
                window.SetView(new View());
                memUsageText.DisplayedString = (Process.GetCurrentProcess().PrivateMemorySize64/1000f)+"kb";
                window.Draw(memUsageText);
                window.Display();
            }
        }
    }
}