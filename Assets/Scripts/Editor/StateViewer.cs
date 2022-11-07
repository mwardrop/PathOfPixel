using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Client.Editor
{
    public class EnemiesItem
    {
        public Guid guid { get; set; }
        public bool selected { get; set; }
    }
    public class PlayersItem
    {
        public int id { get; set; }
        public bool selected { get; set; }
    }


    public class StateViewer : EditorWindow
    {
        protected static bool ShowServerPlayer = true;
        protected static bool ShowLocalPlayer = true;

        protected static bool ShowServerEnemy = true;
        protected static bool ShowLocalEnemy = true;

        protected static bool ShowServerWorld = true;
        protected static bool ShowLocalWorld = true;

        protected static bool ShowSelectEnemy = true;
        protected static bool ShowSelectPlayer = true;

        protected static List<EnemiesItem> Enemies = new List<EnemiesItem>();
        protected static Guid SelectedEnemy;
        protected static List<PlayersItem> Players = new List<PlayersItem>();
        protected static int SelectedPlayer;

        protected static Vector2 Column1Scroll;
        protected static Vector2 Column2Scroll;
        protected static Vector2 Column3Scroll;

        // Add menu item named "My Window" to the Window menu
        [MenuItem("Window/Game State Viewer")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(StateViewer), false, "Game State Viewer");
        }

        void OnGUI()
        {

            if (ClientManager.Instance == null || ServerManager.Instance == null)
            {
                ShowError("Game not running.");
                return;
            }

            if (!ClientManager.Instance.Client.Connected)
            {
                ShowError("Client not connected.");
                return;
            }

            if (ClientManager.Instance.StateManager == null || ServerManager.Instance.StateManager == null)
            {
                ShowError("State Managers not loaded.");
                return;
            }

            if(ClientManager.Instance.StateManager.PlayerState == null)
            {
                ShowError("Player State not loaded.");
            }


                GUILayout.BeginHorizontal(); //side by side columns

                GUILayout.BeginVertical(); //Layout objects vertically in each column

                Column1Scroll = EditorGUILayout.BeginScrollView(Column1Scroll);

                RenderPlayerViewer();

                EditorGUILayout.EndScrollView();

                GUILayout.EndVertical();

                GUILayout.BeginVertical();

                Column2Scroll = EditorGUILayout.BeginScrollView(Column2Scroll);

                RenderEnemyViewer();

                EditorGUILayout.EndScrollView();

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();


        }

        private void RenderPlayerViewer()
        {
  
            ShowSelectPlayer = EditorGUILayout.Foldout(ShowSelectPlayer, "Select Player");
            if (ShowSelectPlayer)
            {
                foreach (PlayerState player in ServerManager.Instance.StateManager.WorldState.Players)
                {
                    if (Players.Count(x => x.id == player.ClientId) == 0)
                    {
                        if (Players.Count() == 0)
                        {
                            Players.Add(new PlayersItem() { id = player.ClientId, selected = true });
                        }
                        else
                        {
                            Players.Add(new PlayersItem() { id = player.ClientId, selected = false });

                        }
                    }
                    Players.First(x => x.id == player.ClientId).selected =
                        EditorGUILayout.Toggle(player.Name, Players.First(x => x.id == player.ClientId).selected);

                    if (Players.First(x => x.id == player.ClientId).selected == true &&
                        Players.First(x => x.id == player.ClientId).id != SelectedPlayer)
                    {
                        Players.First(x => x.selected == true).selected = false;
                        SelectedPlayer = Players.First(x => x.id == player.ClientId).id;

                    }
                }
            }
            EditorGUILayout.LabelField("---------------------------------");

            ShowServerPlayer = EditorGUILayout.Foldout(ShowServerPlayer, "Server Player State");
            if (ShowServerPlayer)
            {
                var serverPlayerState = ServerManager.Instance.Connections[SelectedPlayer].PlayerState;

                foreach (PropertyInfo propertyInfo in ServerManager.Instance.Connections[SelectedPlayer].PlayerState.GetType().GetProperties())
                {
                    if (!(propertyInfo.PropertyType == typeof(List<KeyValueState>))){
                        EditorGUILayout.LabelField(propertyInfo.Name, propertyInfo.GetValue(ServerManager.Instance.Connections[SelectedPlayer].PlayerState).ToString());
                    } else
                    {
                        List<KeyValueState>list = (List<KeyValueState>)propertyInfo.GetValue(
                            ServerManager.Instance.Connections[SelectedPlayer].PlayerState);

                        foreach (KeyValueState kv in list)
                        {
                            EditorGUILayout.LabelField($"{kv.Key}", kv.Value.ToString());
                        }
                    }
                }
            }
            EditorGUILayout.LabelField("---------------------------------");

            ShowLocalPlayer = EditorGUILayout.Foldout(ShowLocalPlayer, "Local Player State");
            if (ShowLocalPlayer)
            {
                foreach (PropertyInfo propertyInfo in ClientManager.Instance.StateManager.WorldState.GetPlayerState(SelectedPlayer).GetType().GetProperties())
                {
                    if (!(propertyInfo.PropertyType == typeof(List<KeyValueState>)))
                    {
                        EditorGUILayout.LabelField(propertyInfo.Name, propertyInfo.GetValue(ClientManager.Instance.StateManager.PlayerState).ToString());
                    } else
                    {
                        List<KeyValueState> list = (List<KeyValueState>)propertyInfo.GetValue(
                            ClientManager.Instance.StateManager.WorldState.GetPlayerState(SelectedPlayer));

                        foreach (KeyValueState kv in list)
                        {
                            EditorGUILayout.LabelField($"{kv.Key}", kv.Value.ToString());
                        }
                    }

                }
            }
            EditorGUILayout.LabelField("---------------------------------");
        }

        private void RenderEnemyViewer()
        {
            ShowSelectEnemy = EditorGUILayout.Foldout(ShowSelectEnemy, "Select Enemy");
            if (ShowSelectEnemy)
            {
                foreach (SceneState scene in ServerManager.Instance.StateManager.WorldState.Scenes)
                {
                    foreach (EnemyState enemy in scene.Enemies)
                    {

                        if (Enemies.Count(x => x.guid == enemy.EnemyGuid) == 0)
                        {
                            if (Enemies.Count() == 0)
                            {
                                Enemies.Add(new EnemiesItem() { guid = enemy.EnemyGuid, selected = true });
                                SelectedEnemy = enemy.EnemyGuid;
                            }
                            else
                            {
                                Enemies.Add(new EnemiesItem() { guid = enemy.EnemyGuid, selected = false });
                            }

                        }
                        Enemies.First(x => x.guid == enemy.EnemyGuid).selected =
                            EditorGUILayout.Toggle(enemy.Name, Enemies.First(x => x.guid == enemy.EnemyGuid).selected);

                        if (Enemies.First(x => x.guid == enemy.EnemyGuid).selected == true &&
                            Enemies.First(x => x.guid == enemy.EnemyGuid).guid != SelectedEnemy)
                        {
                            Enemies.First(x => x.selected == true).selected = false;
                            SelectedEnemy = Enemies.First(x => x.guid == enemy.EnemyGuid).guid;

                        }

                    }
                }
            }
            EditorGUILayout.LabelField("---------------------------------");

            ShowServerEnemy = EditorGUILayout.Foldout(ShowServerEnemy, "Server Enemy State");
            if (ShowServerEnemy)
            {
                foreach (SceneState scene in ServerManager.Instance.StateManager.WorldState.Scenes)
                {
                    foreach (EnemyState enemy in scene.Enemies)
                    {
                        if (enemy.EnemyGuid == SelectedEnemy)
                        {
                            foreach (PropertyInfo propertyInfo in enemy.GetType().GetProperties())
                            {
                                if (!(propertyInfo.PropertyType == typeof(List<KeyValueState>)))
                                {
                                    EditorGUILayout.LabelField(propertyInfo.Name, propertyInfo.GetValue(enemy).ToString());
                                }
                                else
                                {
                                    List<KeyValueState> list = (List<KeyValueState>)propertyInfo.GetValue(
                                        enemy);

                                    foreach (KeyValueState kv in list)
                                    {
                                        var displayKey = kv.Key;
                                        if (displayKey.All(char.IsNumber)) { displayKey = $"{propertyInfo.Name} {displayKey}"; }

                                        EditorGUILayout.LabelField($"{displayKey}", kv.Value.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            EditorGUILayout.LabelField("---------------------------------");

            ShowLocalEnemy = EditorGUILayout.Foldout(ShowLocalEnemy, "Local Enemy State");
            if (ShowLocalEnemy)
            {
                foreach (SceneState scene in ClientManager.Instance.StateManager.WorldState.Scenes)
                {
                    foreach (EnemyState enemy in scene.Enemies)
                    {
                        if (enemy.EnemyGuid == SelectedEnemy)
                        {
                            foreach (PropertyInfo propertyInfo in enemy.GetType().GetProperties())
                            {
                                if (!(propertyInfo.PropertyType == typeof(List<KeyValueState>)))
                                {
                                    EditorGUILayout.LabelField(propertyInfo.Name, propertyInfo.GetValue(enemy).ToString());
                                }
                                else
                                {
                                    List<KeyValueState> list = (List<KeyValueState>)propertyInfo.GetValue(
                                        enemy);

                                    foreach (KeyValueState kv in list)
                                    {
                                        EditorGUILayout.LabelField($"{kv.Key}", kv.Value.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            EditorGUILayout.LabelField("---------------------------------");
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void ShowError(string ex)
        {
            EditorGUILayout.LabelField("Could not connect to Game State.", new GUIStyle() { normal = new GUIStyleState() { textColor = Color.yellow } });
            EditorGUILayout.LabelField(ex.ToString(), new GUIStyle() { normal = new GUIStyleState() { textColor = Color.grey } });

        }
    }
}