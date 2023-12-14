using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamblingDiscordBot
{
    public struct stats
    {
        public int cash;
        public int level;
        public int xp;

        public stats(int cash, int level, int xp)
        {
            this.cash = cash;
            this.level = level;
            this.xp = xp;
        }
    }

    public struct upgrades
    {
        public int NoOfDice;
        public int DieHighestNum;

        public upgrades(int NoOfDice, int DieHighestNum)
        {
            this.NoOfDice = NoOfDice;
            this.DieHighestNum = DieHighestNum;
        }
    }

    public class Player
    {
        ulong id;
        stats playerStats;
        upgrades upgradeLevels;

        public int ID
        {
            get
            {
                return ID;
            }
        }
        public stats PlayerStats
        {
            get 
            { 
                return playerStats;
            }
        }
        public upgrades UpgradeLevels
        {
            get
            {
                return upgradeLevels;
            }
        }

        public Player(ulong PlayerId, stats PlayerStats, upgrades UpgradeLevels) 
        {
            id = PlayerId;
            upgradeLevels = UpgradeLevels;
            playerStats = PlayerStats;
        }
    }
}
