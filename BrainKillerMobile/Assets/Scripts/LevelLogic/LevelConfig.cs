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

        public bool selfCheck()
        {
            if (numOfCol <= 0 || numOfRow <= 0 || showTime <= 0 || imageNames.Length <= 0)
            {
                return false;
            }
            
            if (numOfCol * numOfRow / 2 != imageNames.Length)
            {
                return false;
            }

            return true;
        }
    }

    public struct imageDetectiveLevelConfig
    {
        public LevelConfig normalConfig;
        public string[] imageNames;
        public int numOfRow;
        public int numOfCol;
        public int timeLimit;
    }

    public struct BPCheckLevelConfig
    {
        public LevelConfig normalConfig;
        public string imageName;
        public string swcName;
    }
}