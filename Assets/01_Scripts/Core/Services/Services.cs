using UnityEngine;

namespace AniDrag.Core
{
    /// <summary>
    /// Purpose:
    ///    This is a static class that holds references to the concrete service instances,
    ///    and provides global access to them.
    /// </summary>
    public static class Services
    {
        private static IInputService _input;
        private static IGameStateService _gameState;
        private static ISceneService _scene;
        private static ISettingsService _settings;
        private static IWorldTickService _worldTickService;
        private static IEventBus _eventBus; 

        public static IInputService Input
        {
            get
            {
                if (_input == null)
                    Debug.LogError("Input service not registered!");
                return _input;
            }
        }

        public static IGameStateService GameState
        {
            get
            {
                if (_gameState == null)
                    Debug.LogError("GameState service not registered!");
                return _gameState;
            }
        }

        public static ISceneService Scene
        {
            get
            {
                if (_scene == null)
                    Debug.LogError("Scene service not registered!");
                return _scene;
            }
        }

        public static ISettingsService Settings
        {
            get
            {
                if (_settings == null)
                    Debug.LogError("Settings service not registered!");
                return _settings;
            }
        }
        public static IWorldTickService WorldTick
        {
            get
            {
                if (_worldTickService == null)
                    Debug.LogError("World Tick service not registered!");
                return _worldTickService;
            }
        }
        public static IEventBus EventBus  
        {
            get
            {
                if (_eventBus == null)
                    Debug.LogError("EventBus service not registered!");
                return _eventBus;
            }
        }

        public static void RegisterInput(IInputService service) => _input = service;
        public static void RegisterGameState(IGameStateService service) => _gameState = service;
        public static void RegisterScene(ISceneService service) => _scene = service;
        public static void RegisterSettings(ISettingsService service) => _settings = service;
        public static void RegisterWorldTick(IWorldTickService service) => _worldTickService = service; 
        public static void RegisterEventBus(IEventBus service) => _eventBus = service; 

        public static void Clear()
        {
            _input = null;
            _gameState = null;
            _scene = null;
            _settings = null;
            _worldTickService = null;
            _eventBus = null;
        }
    }

}