using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MRK {
    public class Account {
        const int EQP_NONE = -1;
        const int EQP_WP1 = 0;
        const int EQP_WP2 = 1;
        const int EQP_WP3 = 2;
        const int EQP_HAT = 3;
        const int EQP_SKIN = 4;
        const int EQP_PERK = 5;
        const int EQP_ITM1 = 6;
        const int EQP_ITM2 = 7;
        const int EQP_ITM3 = 8;

        public enum Weapon {
            Bandit = 1,
            Marauder = 2,
            Flusakov = 3,
            Vega = 5,
            Atilla = 6,
            Shredder = 7,
            Bigboy = 9,
            Agony = 10,
            Warmonger = 11,
            Mace = 13,
            Doomhammer = 14,
            Smasher = 15,
            OSOG = 16,
            Surgeon = 17,
            Annihilator = 18,
            Shitstorm = 19,
            ShitstormII = 20,
            Redeemer = 21,
            Spate = 22,
            Barrage = 23,
            Violator = 24
        }

        public enum Skin {
            Soldier = 1,
            Mutant,
            Dancer,
            Assassin,
            Beast,
            Sara,
            Sherif,
            Toltech,
            Widow,
            Slade
        }

        public enum Hat {
            Cylinder = 1,
            Football,
            Halloween,
            Japan,
            Mushroom,
            Pirate,
            Viking,
            Afro,
            Astro,
            Beanie,
            Birthday,
            Headphones,
            Husita,
            Indian,
            Kastrol,
            Student,
            Boar,
            Pilot,
            Skull,
            Warrior,
            Spider,
            Shrapnel,
            Shinobi,
            Madfinger
        }

        public enum Perk {
            ExtendedHealth = 1,
            ExtendedHealthII,
            Sprint,
            SprintII,
            SprintIII,
            FasterMove,
            FasterMoveII,
            FlakJacket,
            FlakJacketII
        }

        public enum Item {
            BoxHealth = 1,
            BoxAmmo = 2,
            GrenadeFrag = 8,
            GrenadeFlash = 9,
            GrenadeEMP = 10,
            Mine = 11,
            SpiderHuman = 12,
            SpiderEmp = 13,
            SentryGun = 14,
            SentryGunRail = 15,
            SentryGunRockets = 16,
            BoxHealthII = 17,
            BoxAmmoII = 18,
            EnemyDetector = 19,
            EnemyDetectorII = 20,
            Jammer = 21,
            GrenadeEMPII = 22,
            MineEMP = 23,
            MineEMPII = 24,
            GrenadeFragII = 25,
            SpiderEmpII = 26,
            SentryGunII = 27,
            GrenadeSting = 28,
            BoosterSpeed = 29,
            BoosterAccuracy = 30,
            BoosterArmor = 31,
            BoosterDamage = 32,
            BoosterInvicible = 33
        }

        bool m_Dirty;
        List<string> m_ReusableBuffer;
        List<int> m_ShiftBuffer;

        public bool IsDirty {
            get {
                return m_Dirty;
            }

            set {
                m_Dirty = value;

                Program.MainInstance.UpdateProjectButtons();
            }
        }

        public string Nickname { get; private set; }
        public string Username { get; private set; }
        public List<Weapon> Weapons { get; private set; }
        public List<Skin> Skins { get; private set; }
        public List<Hat> Hats { get; private set; }
        public List<Perk> Perks { get; private set; }
        public List<Item> Items { get; private set; }
        public List<int> Equip { get; private set; }
        public bool Premium { get; private set; }

        /* NATIVE CPP IMPL
        Player newplayer = Player {
		    username,
		    nickname,
		    false,
		    10000,
		    0,
		    0,
		    1,
		    {
		    	WEAPON_BANDIT
		    },
		    {
		    	SKIN_SOLDIER,
		    	SKIN_MUTANT
		    },
		    {
		    },
		    {
		    	PERK_SPRINT
		    },
		    {
		    	ITEM_FRAGGRENADE
		    },
		    {
			    WEAPON_BANDIT,		//wp1
			    EQUIP_NOTHING,		//wp2
			    EQUIP_NOTHING,		//wp3
    
			    EQUIP_NOTHING,		//hat

			    SKIN_SOLDIER,		//skin

			    PERK_SPRINT,		//perk

			    ITEM_FRAGGRENADE,	//itm1
			    EQUIP_NOTHING,		//itm2
			    EQUIP_NOTHING		//itm3
		    }
	    };
        */

        public static Account Create(string user, string nick) {
            return new Account {
                Nickname = nick,

                Username = user,

                Weapons = new List<Weapon>() { 
                    Weapon.Bandit
                },

                Skins = new List<Skin>() { 
                    Skin.Soldier,
                    Skin.Mutant
                },

                Hats = new List<Hat>(),

                Perks = new List<Perk>() {
                    Perk.Sprint,
                },

                Items = new List<Item>() {
                    Item.GrenadeFrag
                },

                Equip = new List<int>() {
                    (int)Weapon.Bandit,
                    EQP_NONE,
                    EQP_NONE,

                    EQP_NONE,

                    (int)Skin.Soldier,

                    (int)Perk.Sprint,

                    (int)Item.GrenadeFrag,
                    EQP_NONE,
                    EQP_NONE
                },

                Premium = false,

                m_ReusableBuffer = new List<string>(),
                m_ShiftBuffer = new List<int>()
            };
        }

        public void SetPremium(bool hasPremium) {
            IsDirty = true;
            Premium = hasPremium;
        }

        public void SetNickname(string nickname) {
            IsDirty = true;
            Nickname = nickname;
        }

        void GetEqInfo(int section, out int min, out int max, out Type enm) {
            min = 0;
            max = -1;
            enm = null;

            switch (section) {

                case 5:
                    enm = typeof(Weapon);
                    min = EQP_WP1;
                    max = EQP_WP3;
                    break;

                case 6:
                    enm = typeof(Skin);
                    min = max = EQP_SKIN;
                    break;

                case 7:
                    enm = typeof(Hat);
                    min = max = EQP_HAT;
                    break;

                case 8:
                    enm = typeof(Perk);
                    min = max = EQP_PERK;
                    break;

                case 9:
                    enm = typeof(Item);
                    min = EQP_ITM1;
                    max = EQP_ITM3;
                    break;

            }
        }

        public void GetEquip(List<string> buffer, int section, bool clear = false) {
            if (buffer.Count > 0)
                buffer.Clear();

            switch (section) {

                case 5:
                    Weapons.ForEach(x => buffer.Add(x.ToString()));
                    break;

                case 6:
                    Skins.ForEach(x => buffer.Add(x.ToString()));
                    break;

                case 7:
                    Hats.ForEach(x => buffer.Add(x.ToString()));
                    break;

                case 8:
                    Perks.ForEach(x => buffer.Add(x.ToString()));
                    break;

                case 9:
                    Items.ForEach(x => buffer.Add(x.ToString()));
                    break;

            }

            int min;
            int max;
            Type enm;
            GetEqInfo(section, out min, out max, out enm);

            int sz = max - min;
            if (sz < 0 || enm == null)
                return;

            if (buffer == m_ReusableBuffer || clear)
                buffer.Clear();

            for (int i = 0; i <= sz; i++) {
                int eidx = Equip[min + i];
                string str = eidx != EQP_NONE ? Enum.GetName(enm, Equip[min + i]) : "None";

                if (str != null && !buffer.Contains(str))
                    buffer.Add(str);
            }
        }

        string ConcatenateList<T>(List<T> list) {
            string str = "";

            int idx = 0;
            while (idx < list.Count) {
                T item = list[idx++];

                if (item == null)
                    str += "None";
                else
                    str += item;

                if (idx < list.Count)
                    str += ", ";
            }

            return str;
        }

        public string GetCtxItems(int section) {
            switch (section) {

                case 0:
                    return ConcatenateList<Weapon>(Weapons);

                case 1:
                    return ConcatenateList<Skin>(Skins);

                case 2:
                    return ConcatenateList<Hat>(Hats);

                case 3:
                    return ConcatenateList<Perk>(Perks);

                case 4:
                    return ConcatenateList<Item>(Items);

            }

            if (section < 5 || section > 9)
                return "";

            m_ReusableBuffer.Clear();
            GetEquip(m_ReusableBuffer, section);

            return ConcatenateList<string>(m_ReusableBuffer);
        }

        void ProcessAction<T>(List<T> owner, T item, bool action) {
            if (owner == null)
                return;

            if (action && !owner.Contains(item))
                owner.Add(item);

            if (!action && owner.Contains(item))
                owner.Remove(item);
        }

        T AsEnum<T>(string item) {
            return (T)Enum.Parse(typeof(T), item);
        }

        public void ModifyCtx(int section, string eq, bool action) {
            if (section < 5) {
                switch (section) {

                    case 0:
                        ProcessAction<Weapon>(Weapons, AsEnum<Weapon>(eq), action);
                        break;

                    case 1:
                        ProcessAction<Skin>(Skins, AsEnum<Skin>(eq), action);
                        break;

                    case 2:
                        ProcessAction<Hat>(Hats, AsEnum<Hat>(eq), action);
                        break;

                    case 3:
                        ProcessAction<Perk>(Perks, AsEnum<Perk>(eq), action);
                        break;

                    case 4:
                        ProcessAction<Item>(Items, AsEnum<Item>(eq), action);
                        break;
                }

                return;
            }

            int min;
            int max;
            Type enm;
            GetEqInfo(section, out min, out max, out enm);

            int sz = max - min;

            m_ReusableBuffer.Clear();
            GetEquip(m_ReusableBuffer, section);

            if (sz == 0)
                m_ReusableBuffer.Clear();

            ProcessAction<string>(m_ReusableBuffer, eq, action);

            if (sz == 0 && !action)
                m_ReusableBuffer.Clear();

            while (m_ReusableBuffer.Count <= sz)
                m_ReusableBuffer.Add("None");

            m_ShiftBuffer.Clear();

            for (int i = 0; i <= sz; i++) { //shift
                string current = m_ReusableBuffer[i];
                if (current == null || current.Length == 0 || current == "None") {
                    m_ShiftBuffer.Add(i);
                    continue;
                }

                if (m_ShiftBuffer.Count > 0) {
                    m_ReusableBuffer[m_ShiftBuffer[0]] = current;
                    m_ReusableBuffer[i] = "None";
                    m_ShiftBuffer.RemoveAt(0);
                }
            }

            for (int i = 0; i <= sz; i++) {
                string item = m_ReusableBuffer[i];

                Equip[i + min] = item == "None" ? EQP_NONE : (int)Enum.Parse(enm, item);
            }
        }
    }
}
