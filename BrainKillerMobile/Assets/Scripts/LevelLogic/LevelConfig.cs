namespace LevelLogic
{
    public struct ModeConfig
    {
        public string modeName;
        public string modeDescription;
        public int numOfLevels;
    }
    
    
    public struct LevelConfig
    {
        public string modeName;
        public int levelId;
        public string levelName;
        public string levelDescription;
    }

    public struct flipLevelConfig
    {
        public LevelConfig normalConfig;
        public string fullImgFrontName;
        public string fullImgBackName;
        public int numOfItem;
    }
    
    public struct matchCardsLevelConfig
    {
        public LevelConfig normalConfig;
        public string[] imageNames;
        public int showTime;
        public int numOfRow;
        public int numOfCol;
    }
}