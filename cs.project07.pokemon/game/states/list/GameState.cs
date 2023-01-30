﻿using cs.project07.pokemon.game.map;
using cs.project07.pokemon.game.save;
using cs.project07.pokemon.game.entites;
using cs.project07.pokemon.game.states.gui.managers;
using cs.project07.pokemon.game.states.gui;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace cs.project07.pokemon.game.states.list
{
    public class GameState : State
    {
        public Map CurrentMap;
        private Dictionary<string, Map> Maps;

        public Player Player { get; set; }
        private CombatState Combat;
        private Game game;

        private ButtonManager _buttonManager;
        private Dictionary<string, Button> _buttons;
        private DialogBox _dialogBox;
        bool showMenu = false;
        public GameState(Game gameReceive) : base(gameReceive)
        {
            game = gameReceive;
            Init();
        }

        protected override void Init()
        {
            Name = "Pokemon";
            InitMap();
            InitPlayer();
            _dialogBox = new DialogBox(this);
            InitMenu();
        }

        private void InitMenu()
        {
            _buttonManager = new ButtonManager();
            _buttons = _buttonManager.Buttons;
            _buttons["INVENTORY"] = new Button(_dialogBox, "Inventory")
            {
                Selected = true,
                Offsets = new Vector2(150, 0),
                Action = () =>
                {
                    //Game.StatesList?.Push(new InventoryState(Parent));
                }
            };
            _buttons["SAVE"] = new Button(_dialogBox, "Save")
            {
                Selected = false,
                Offsets = new Vector2(150, 0),
                Action = () =>
                {
                    //TO DO SAVE THE GAME
                }
            };
            _buttons["EXIT"] = new Button(_dialogBox, "Exit")
            {
                Selected = false,
                Offsets = new Vector2(150, 0),
                Action = () =>
                {
                    Game.StatesList?.Pop();
                }
            };

            _buttonManager.InitHandleKeyEvent();

            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons.ElementAt(i).Value.Offsets += new Vector2(3, 1 + i);
            }
        }

        private void InitMap()
        {
            Maps = new Dictionary<string, Map>
            {
                { "map1", new Map(this,"map1") },
                { "map2", new Map(this,"map2") }
            };

            Maps["map1"].ParseFileToLayers("game/map/list/Map1.txt");
            Maps["map2"].ParseFileToLayers("game/map/list/Map2.txt");
            CurrentMap = Maps["map1"];
        }

        private void InitPlayer()
        {
            Player = new Player(CurrentMap.PlayerSpawnPosition, this);
            
            CurrentMap.Zoom = 4;
            Player.zoomPlayer(CurrentMap.Zoom);
        }

        public override void HandleKeyEvent(ConsoleKey pressedKey)
        {
            if(CurrentMap != null)
            {
                if(pressedKey == ConsoleKey.Escape)
                    HandleKeyEventButtons(pressedKey);
                switch (pressedKey)
                {
                    case ConsoleKey.Insert:
                        // TODO Remove when Pause menu is complete
                        // Back to previous menu
                        Game.StatesList?.Pop();
                        break;
                    case ConsoleKey.UpArrow:
                        // TODO Player move up
                        if (Player.collisionWall(Map.Layers["WALL"].ZoomedData, 'N') == true 
                            && Map.Zoom == 4 && !showMenu)
                        {
                            Player.mouvPlayer('N');
                            mouv = true;
                        }
                        if(showMenu) 
                            Button.SelectPrevious(_buttonManager.Buttons); 
                        break;
                    case ConsoleKey.DownArrow:
                        // TODO Player move down
                        if (Player.collisionWall(Map.Layers["WALL"].ZoomedData, 'S') == true 
                            && Map.Zoom == 4 && !showMenu)
                        {
                            Player.mouvPlayer('S');
                            mouv = true;
                        }
                        if(showMenu)
                            Button.SelectNext(_buttonManager.Buttons);
                        break;
                    case ConsoleKey.LeftArrow:
                        // TODO Player move left
                        if (Player.collisionWall(Map.Layers["WALL"].ZoomedData, 'O') == true 
                            && Map.Zoom == 4 && !showMenu)
                        {
                            Player.mouvPlayer('O');
                            mouv = true;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        // TODO Player move right
                        if (Player.collisionWall(Map.Layers["WALL"].ZoomedData, 'E') == true 
                            && Map.Zoom == 4 && !showMenu)
                        {
                            Player.mouvPlayer('E');
                            mouv = true;
                        }
                        break;
                    case ConsoleKey.Enter:
                        // TODO Player use action
                        if(showMenu)
                            Button.ExecuteAction(_buttonManager.Buttons);
                        break;
                    case ConsoleKey.M:
                        if (Map.Zoom == 4 && !showMenu)
                        {
                            CurrentMap.Zoom = 1;
                            Player.zoomPlayer(CurrentMap.Zoom);
                        }
                        else if (Map.Zoom == 1 && !showMenu)
                        {
                            CurrentMap.Zoom = 4;
                            Player.zoomPlayer(CurrentMap.Zoom);
                        }
                        break;
                }

                if (mouv != true) return;
                Player.collisionGrass(CurrentMap.Layers["GRASS"].ZoomedData, game);
                Player.collisionTeleporter(CurrentMap._Teleporters);
                if(mouv == true)
                    Player.collisionGrass(CurrentMap.Layers["GRASS"].ZoomedData, game);
            }
        }

        private void HandleKeyEventButtons(ConsoleKey pressedKey)
        {
            showMenu = !showMenu;
            _buttonManager.HandleKeyEvent(pressedKey);
        }

        public override void Update()
        {
            //base.Update();

            // Update childs
            // ------ Map
            Map?.Update();
            if(showMenu == true)
                _buttonManager.Update();
        }

        public override void Render()
        {
            //base.Render();

            // Render childs
            // ------ Map
            Map?.Render();
            if(showMenu == true)
                _buttonManager?.Render();
            
            Player.drawPlayer();
        }
    }
}
