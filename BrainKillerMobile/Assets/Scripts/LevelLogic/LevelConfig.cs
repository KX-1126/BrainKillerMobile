namespace LevelLogic
{
    public struct ModeConfig
    {
        public string modeName;
        public string modeDescription;
        public int numOfLevels;
    }
    
    [System.Serializable]
    public struct LevelConfig
    {
        public string modeName;
        public int levelId;
        public string levelName;
        public string levelDescription;
    }
    
    [System.Serializable]
    public struct flipLevelConfig
    {
        public LevelConfig normalConfig;
        public string frontImageName;
        public string backImageName;
        public int numOfItem;
    }
    
    [System.Serializable]
    public struct matchCardsLevelConfig
    {
        public LevelConfig normalConfig;
        public string imageNames;
        public int showTime;
        public int numOfRow;
        public int numOfCol;

        public bool selfCheck()
        {
            if (numOfCol <= 0 || numOfRow <= 0 || showTime <= 0 || imageNames.Length <= 0)
            {
                return false;
            }
            
            if (numOfCol * numOfRow / 2 != imageNames.Split(",").Length)
            {
                return false;
            }

            return true;
        }
    }

    [System.Serializable]
    public struct imageDetectiveLevelConfig
    {
        public LevelConfig normalConfig;
        public string imageNames;
        public int numOfRow;
        public int numOfCol;
        public int timeLimit;
    }

    [System.Serializable]
    public struct BPCheckLevelConfig
    {
        public LevelConfig normalConfig;
        public int swcId;
        public string imageName;
        public string swcName;
    }
}