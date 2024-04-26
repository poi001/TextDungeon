using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Markup;
using static System.Formats.Asn1.AsnWriter;

namespace TextDeongeon
{
    internal class Program
    {
        static Character character = new Character();
        static List<Item> storeItem = new List<Item>();

        enum Scene : byte
        {
            Home,
            Stat,
            Inven,
            Equipment,
            Store,
            Dungeon,
            Dungeon_Easy_Success,
            Dungeon_Normal_Success,
            Dungeon_Hard_Success,
            Dungeon_Easy_Fail,
            Dungeon_Normal_Fail,
            Dungeon_Hard_Fail,
            Heal,
            Dead
        }

        static void HomeScene()
        {
HomeSceneStart:
            Console.Clear();
            Console.WriteLine("마을입니다.");
            Console.WriteLine("던전으로 들어가기 전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 들어가기");
            Console.WriteLine("5. 휴식하기");
            Console.WriteLine("6. 저장하기");
            Console.WriteLine("7. 저장파일 삭제");
            Console.WriteLine();
            Console.WriteLine("0. 게임 종료");
            Console.WriteLine();
SavePoint:
            Console.WriteLine("가고 싶은 장소를 입력하십시오,");

            switch (SelectScene(7))
            {
                case 1:
                    {
                        ChanageScene(Scene.Stat);
                        break;
                    }
                case 2:
                    {
                        ChanageScene(Scene.Inven);
                        break;
                    }
                case 3:
                    {
                        ChanageScene(Scene.Store);
                        break;
                    }
                case 4:
                    {
                        ChanageScene(Scene.Dungeon);
                        break;
                    }
                case 5:
                    {
                        ChanageScene(Scene.Heal);
                        break;
                    }
                case 6:
                    {
                        SaveGame();
                        Console.WriteLine("저장완료");
                        goto SavePoint;
                    }
                case 7:
                    {
                        DeleteSaveFile();
                        Console.WriteLine("저장파일 삭제완료");
                        goto SavePoint;
                    }


                case 0:
                    {
                        Console.WriteLine("게임이 종료되었습니다.");
                        return;
                    }
                default:
                    break;
            }

            goto HomeSceneStart;

        }

        static void StatScene()
        {
            Console.Clear();
            Console.WriteLine("-상태창-");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine();
            Console.WriteLine("Level  : {0}", character.level);
            Console.WriteLine("이름   : {0}", character.name);
            Console.WriteLine("직업   : {0}", character.characterClass);
            if(character.extraAttackDamage > 0) Console.WriteLine("공격력 : {0} + ({1})", character.attackDamage, character.extraAttackDamage);
            else Console.WriteLine("공격력 : {0}", character.attackDamage);
            if (character.extraDefense > 0) Console.WriteLine("방어력 : {0} + ({1})", character.defense, character.extraDefense);
            else Console.WriteLine("방어력 : {0}", character.defense);
            Console.WriteLine("체력   : {0}", character.health);
            Console.WriteLine("골드   : {0}G", character.gold);
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("가고 싶은 장소를 입력하십시오,");

            SelectScene(0);
        }

        static void InvenScene()
        {
InvenSceneStart:
            Console.Clear();
            Console.WriteLine("-인벤토리-");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < character.list.Count; i++)
            {
                var _item = character.list[i];
                if (_item == character.weapon || _item == character.armor)
                {
                    Console.WriteLine("-[E]{0} | {1} | {2}", _item.name, _item.statText, _item.description);
                }
                else
                {
                    Console.WriteLine("-{0} | {1} | {2}", _item.name, _item.statText, _item.description);
                }
            }
            Console.WriteLine();
            Console.WriteLine("1. 장비 관리");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 선택해주십시오.");

            switch(SelectScene(1))
            {
                case 1:
                    {
                        ChanageScene(Scene.Equipment);
                        goto InvenSceneStart;
                    }
                default:
                    break;
            }

        }

        static void EquipmentScene()
        {
EquipmentSceneStart:
            Console.Clear();
            Console.WriteLine("-인벤토리-");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            for (int i = 0; i < character.list.Count; i++)
            {
                var _item = character.list[i];
                if(_item == character.weapon || _item == character.armor)
                {
                    Console.WriteLine("{0}. [E]{1} | {2} | {3}", i + 1, _item.name, _item.statText, _item.description);
                }
                else
                {
                    Console.WriteLine("{0}. {1} | {2} | {3}", i + 1, _item.name, _item.statText, _item.description);
                }
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 선택해주십시오.");

            int count = character.list.Count;
            int inputNum = SelectScene(in count);
            var item = character.list;

            if ( inputNum != 0 )
            {

                var it = item[inputNum - 1];

                if(it.IsWeapon)
                {

                    if (character.weapon != it)
                    {

                        character.EquipWeapon(ref it);
                        goto EquipmentSceneStart;

                    }
                    else
                    {

                        character.TakOffWeapon();
                        goto EquipmentSceneStart;

                    }

                }
                else
                {

                    if (character.armor != it)
                    {

                        character.EquipArmor(ref it);
                        goto EquipmentSceneStart;

                    }
                    else
                    {

                        character.TakOffArmor();
                        goto EquipmentSceneStart;

                    }

                }

            }

        }

        static void StoreScene()
        {
            bool buyTextOn = false;

        StoreSceneStart:
            Console.Clear();
            Console.WriteLine("-상점-");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            Console.WriteLine("{0} G", character.gold);
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < storeItem.Count; i++) 
            {
                var _item = storeItem[i];

                if (character.list.Contains(_item))
                {
                    Console.WriteLine("{0}. {1} | {2} | {3} l 구매 완료", i + 1, _item.name, _item.statText, _item.description);
                }
                else
                {
                    Console.WriteLine("{0}. {1} | {2} | {3} ㅣ {4} G", i + 1, _item.name, _item.statText, _item.description, _item.cost);
                }
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            if(buyTextOn) Console.WriteLine("구매 완료!");
        Buy:
            Console.WriteLine("원하시는 행동을 선택해주십시오.");

            int count = storeItem.Count;
            int inputNum = SelectScene(in count);

            if(inputNum != 0)
            {
                if ( character.list.Contains( storeItem[inputNum - 1] ) )
                {
                    Console.WriteLine("이미 구매한 장비입니다.");
                    goto Buy;
                }
                else
                {
                    if( character.gold >= storeItem[inputNum - 1].cost )
                    {
                        character.list.Add(storeItem[inputNum - 1]);
                        character.gold -= storeItem[inputNum - 1].cost;

                        buyTextOn = true;

                        goto StoreSceneStart;
                    }
                    else
                    {
                        Console.WriteLine("골드가 부족합니다.");
                        goto Buy;
                    }
                }
            }
        }

        static void DungeonScene()
        {
            int easyDefense = 5;
            int normalDefense = 11;
            int hardDefense = 17;

            Console.Clear();
            Console.WriteLine("-던전입장-");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[공격력]: {0}", character.attackDamage);
            Console.WriteLine("[방어력]: {0}", character.defense);
            Console.WriteLine();
            Console.WriteLine("1. 쉬운 던전     | 방어력 {0} 이상 권장", easyDefense);
            Console.WriteLine("2. 일반 던전     | 방어력 {0} 이상 권장", normalDefense);
            Console.WriteLine("3. 어려운 던전    | 방어력 {0} 이상 권장", hardDefense);
            Console.WriteLine("");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 선택해주십시오.");

            switch( SelectScene(3) )
            {
                case 1:
                    {
                        if(character.defense < easyDefense)
                        {
                            if (CalDefenseForDungeon()) ChanageScene(Scene.Dungeon_Easy_Success);
                            else ChanageScene(Scene.Dungeon_Easy_Fail);
                        }
                        else ChanageScene(Scene.Dungeon_Easy_Success);
                        break;
                    }
                case 2:
                    {
                        if (character.defense < normalDefense)
                        {
                            if (CalDefenseForDungeon()) ChanageScene(Scene.Dungeon_Normal_Success);
                            else ChanageScene(Scene.Dungeon_Normal_Fail);
                        }
                        else ChanageScene(Scene.Dungeon_Normal_Success);
                        break;
                    }
                case 3:
                    {
                        if (character.defense < hardDefense)
                        {
                            if (CalDefenseForDungeon()) ChanageScene(Scene.Dungeon_Hard_Success);
                            else ChanageScene(Scene.Dungeon_Hard_Fail);
                        }
                        else ChanageScene(Scene.Dungeon_Hard_Success);
                        break;
                    }
            }

        }

        static void Dungeon_EasyScene_Success()
        {
            int difficulty = 1;
            int recommendedDefense = 5;

            Console.Clear();
            Console.WriteLine("던전 클리어");
            Console.WriteLine("축하합니다!!");
            Console.WriteLine("쉬운 던전을 클리어 하였습니다.");
            Console.WriteLine();
            Console.WriteLine("[탐험 결과]");
            Console.WriteLine("체력 {0} -> {1}", character.health, character.health -= CalHealthForDungeon(recommendedDefense));
            Console.WriteLine("Gold {0} G -> {1} G", character.gold, character.gold += DungeonCredit(difficulty));
            Console.WriteLine();
            Console.WriteLine("경험치 + 1");
            Console.WriteLine("현재 경험치 : {0} / {1} -> {2} / {3}", character.currentEXP, character.maxEXP, character.currentEXP + 1, character.maxEXP);
            character.AddEXP();
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            SelectScene(0);

            if (character.health <= 0) ChanageScene(Scene.Dead);
        }

        static void Dungeon_NormalScene_Success()
        {
            int difficulty = 2;
            int recommendedDefense = 11;

            Console.Clear();
            Console.WriteLine("던전 클리어");
            Console.WriteLine("축하합니다!!");
            Console.WriteLine("일반 던전을 클리어 하였습니다.");
            Console.WriteLine();
            Console.WriteLine("[탐험 결과]");
            Console.WriteLine("체력 {0} -> {1}", character.health, character.health -= CalHealthForDungeon(recommendedDefense));
            Console.WriteLine("Gold {0} G -> {1} G", character.gold, character.gold += DungeonCredit(difficulty));
            Console.WriteLine();
            Console.WriteLine("경험치 + 1");
            Console.WriteLine("현재 경험치 : {0} / {1} -> {2} / {3}", character.currentEXP, character.maxEXP, character.currentEXP + 1, character.maxEXP);
            character.AddEXP();
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            SelectScene(0);

            if (character.health <= 0) ChanageScene(Scene.Dead);
        }
        static void Dungeon_HardScene_Success()
        {
            int difficulty = 3;
            int recommendedDefense = 17;

            Console.Clear();
            Console.WriteLine("던전 클리어");
            Console.WriteLine("축하합니다!!");
            Console.WriteLine("어려운 던전을 클리어 하였습니다.");
            Console.WriteLine();
            Console.WriteLine("[탐험 결과]");
            Console.WriteLine("체력 {0} -> {1}", character.health, character.health -= CalHealthForDungeon(recommendedDefense));
            Console.WriteLine("Gold {0} G -> {1} G", character.gold, character.gold += DungeonCredit(difficulty));
            Console.WriteLine();
            Console.WriteLine("경험치 + 1");
            Console.WriteLine("현재 경험치 : {0} / {1} -> {2} / {3}", character.currentEXP, character.maxEXP, character.currentEXP + 1, character.maxEXP);
            character.AddEXP();
            Console.WriteLine();
            Console.WriteLine("0.나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            SelectScene(0);

            if (character.health <= 0) ChanageScene(Scene.Dead);
        }

        static void Dungeon_EasyScene_Fail()
        {
            Console.Clear();
            Console.WriteLine("던전 실패");;
            Console.WriteLine("쉬운 던전을 실패하셨습니다.");
            Console.WriteLine();
            Console.WriteLine("[탐험 결과]");
            Console.WriteLine("체력 {0} -> {1}", character.health, character.health /= 2);
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            SelectScene(0);

            if (character.health <= 0) ChanageScene(Scene.Dead);
        }

        static void Dungeon_NormalScene_Fail()
        {
            Console.Clear();
            Console.WriteLine("던전 실패"); ;
            Console.WriteLine("일반 던전을 실패하셨습니다.");
            Console.WriteLine();
            Console.WriteLine("[탐험 결과]");
            Console.WriteLine("체력 {0} -> {1}", character.health, character.health /= 2);
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            SelectScene(0);

            if (character.health <= 0) ChanageScene(Scene.Dead);
        }
        static void Dungeon_HardScene_Fail()
        {
            Console.Clear();
            Console.WriteLine("던전 실패"); ;
            Console.WriteLine("어려운 던전을 실패하셨습니다.");
            Console.WriteLine();
            Console.WriteLine("[탐험 결과]");
            Console.WriteLine("체력 {0} -> {1}", character.health, character.health /= 2);
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            SelectScene(0);

            if (character.health <= 0) ChanageScene(Scene.Dead);
        }

        static void HealScene()
        {
            Console.Clear();
            Console.WriteLine("-휴식하기-"); ;
            Console.WriteLine("500 G 를 내면 체력을 회복할 수 있습니다. (체력: {0}, 보유 골드: {1} G)", character.health, character.gold);
            Console.WriteLine();
            Console.WriteLine("1. 휴식하기");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
NotEnoughGold:
            if(SelectScene(1) == 1)
            {
                if(character.gold >= 500)
                {
                    character.health = 100;
                    character.gold -= 500;

                    Console.Clear();
                    Console.WriteLine("-휴식 완료-"); ;
                    Console.WriteLine("체력이 모두 회복되었습니다. (체력: {0}, 보유골드: {1} )", character.health, character.gold);
                    Console.WriteLine();
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine();
                    Console.WriteLine("원하시는 행동을 입력해주세요.");

                    SelectScene(0);
                }
                else
                {
                    Console.WriteLine("돈이 부족합니다.");
                    goto NotEnoughGold;
                }
            }

        }

        static void Dead()
        {
            Console.Clear();
            Console.WriteLine("{0} 사망...", character.name); 
            Console.WriteLine("체력이 0이 되어 사망하셨습니다.");
            Console.WriteLine();
            Console.WriteLine("Level  : {0}", character.level);
            Console.WriteLine("이름   : {0}", character.name);
            Console.WriteLine("직업   : {0}", character.characterClass);
            if (character.extraAttackDamage > 0) Console.WriteLine("공격력 : {0} + ({1})", character.attackDamage, character.extraAttackDamage);
            else Console.WriteLine("공격력 : {0}", character.attackDamage);
            if (character.extraDefense > 0) Console.WriteLine("방어력 : {0} + ({1})", character.defense, character.extraDefense);
            else Console.WriteLine("방어력 : {0}", character.defense);
            Console.WriteLine("골드   : {0}G", character.gold);
            Console.WriteLine();
            Console.WriteLine("아무 키나 눌러 게임을 끝내십쇼.");

            if (Console.ReadLine() != null) return;
        }

        static void ChanageScene(in Scene changeScene)
        {
            switch (changeScene)
            {
                case Scene.Home:
                    {
                        HomeScene();
                        break;
                    }
                case Scene.Stat:
                    {
                        StatScene();
                        break;
                    }
                case Scene.Inven:
                    {
                        InvenScene();
                        break;
                    }
                case Scene.Equipment:
                    {
                        EquipmentScene();
                        break;
                    }
                case Scene.Store:
                    {
                        StoreScene();
                        break;
                    }
                case Scene.Dungeon:
                    {
                        DungeonScene();
                        break;
                    }
                case Scene.Dungeon_Easy_Success:
                    {
                        Dungeon_EasyScene_Success();
                        break;
                    }
                case Scene.Dungeon_Normal_Success:
                    {
                        Dungeon_NormalScene_Success();
                        break;
                    }
                case Scene.Dungeon_Hard_Success:
                    {
                        Dungeon_HardScene_Success();
                        break;
                    }
                case Scene.Dungeon_Easy_Fail:
                    {
                        Dungeon_EasyScene_Fail();
                        break;
                    }
                case Scene.Dungeon_Normal_Fail:
                    {
                        Dungeon_NormalScene_Fail();
                        break;
                    }
                case Scene.Dungeon_Hard_Fail:
                    {
                        Dungeon_HardScene_Fail();
                        break;
                    }
                case Scene.Heal:
                    {
                        HealScene();
                        break;
                    }
                case Scene.Dead:
                    {
                        Dead();
                        break;
                    }

                default:
                    break;
            }
        }

        static int SelectScene(in int sceneNum)
        {
            int num;

            while (true)
            {
                bool isPsv = int.TryParse(Console.ReadLine(), out num);

                if (isPsv)
                {
                    if (num < 0 || num > sceneNum) Console.WriteLine("잘못된 입력입니다");
                    else break;
                }
                else Console.WriteLine("잘못된 입력입니다");

            }

            return num;
        }

        static void ItemSetting()
        {
            Item armor1 = new Item();
            Item armor2 = new Item();
            Item armor3 = new Item();
            Item weapon1 = new Item();
            Item weapon2 = new Item();
            Item weapon3 = new Item();

            storeItem.Add(armor1);
            storeItem.Add(armor2);
            storeItem.Add(armor3);
            storeItem.Add(weapon1);
            storeItem.Add(weapon2);
            storeItem.Add(weapon3);

            //armor1, 무쇠 갑옷
            armor1.name = "무쇠 갑옷";
            armor1.status = 5;
            armor1.statText = $"방어력 + {armor1.status}";
            armor1.description = "무쇠로 만들어져 튼튼한 갑옷입니다.";
            armor1.IsWeapon = false;
            armor1.cost = 400;

            //armor2, 젤리 갑옷
            armor2.name = "젤리 갑옷";
            armor2.status = 10;
            armor2.statText = $"방어력 + {armor2.status}";
            armor2.description = "젤리처럼 말랑해 모든 공격이 흡수됩니다.";
            armor2.IsWeapon = false;
            armor2.cost = 1000;

            //armor3, 비브라늄 갑옷
            armor3.name = "비브라늄 갑옷";
            armor3.status = 20;
            armor3.statText = $"방어력 + {armor3.status}";
            armor3.description = "비브라늄으로 만들어졌다. 기차에 치여도 끄떡없다.";
            armor3.IsWeapon = false;
            armor3.cost = 3000;

            //weapon1, 낡은 검
            weapon1.name = "낡은 검";
            weapon1.status = 2;
            weapon1.statText = $"공격력 + {weapon1.status}";
            weapon1.description = "쉽게 볼 수 있는 낡은 검 입니다.";
            weapon1.IsWeapon = true;
            weapon1.cost = 150;

            //weapon2, 스파르타 창
            weapon2.name = "스파르타 창";
            weapon2.status = 7;
            weapon2.statText = $"공격력 + {weapon2.status}";
            weapon2.description = "스파르타의 전사들이 사용했다는 전설의 창입니다.";
            weapon2.IsWeapon = true;
            weapon2.cost = 700;

            //weapon3, AK47
            weapon3.name = "AK47";
            weapon3.status = 20;
            weapon3.statText = $"공격력 + {weapon3.status}";
            weapon3.description = "외할머니의 목숨을 앗아간 무기이다. 이거 진짜 총이다.";
            weapon3.IsWeapon = true;
            weapon3.cost = 2800;

        }

        static int DungeonCredit(int difficulty)
        {
            int credit = 0;
            if (difficulty == 1) credit = 1000;
            else if (difficulty == 2) credit = 1700;
            else if (difficulty == 3) credit = 2500;

            Random random = new Random();
            int extraCredit = random.Next(character.attackDamage, character.attackDamage * 2);
            credit += (int)( credit * ( extraCredit * 0.01f ) );

            return credit;
        }

        static int CalHealthForDungeon(int recommendedDefense)
        {
            int min = 20 + (recommendedDefense - character.defense);
            int max = 35 + (recommendedDefense - character.defense);

            Random random = new Random();

            return random.Next(min, max);
        }

        static bool CalDefenseForDungeon()
        {
            Random random = new Random();
            if (random.Next(0, 100) <= 40) return false;

            return true;
        }

        static void SaveGame()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string relativePath = @"..\..\..\..\save\SaveText";
            string textFile = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

            if (File.Exists(textFile))
            {
                File.Delete(textFile);
            }

            using (StreamWriter sw = File.CreateText(textFile))
            {
                sw.WriteLine(character.level);
                sw.WriteLine(character.currentEXP);
                sw.WriteLine(character.maxEXP);
                sw.WriteLine(character.name);
                sw.WriteLine(character.characterClass);
                sw.WriteLine(character.attackDamage);
                sw.WriteLine(character.extraAttackDamage);
                sw.WriteLine(character.defense);
                sw.WriteLine(character.extraDefense);
                sw.WriteLine(character.health);
                sw.WriteLine(character.gold);

                foreach (var item in character.list)
                {
                    sw.WriteLine(item.name);
                }
            }
        }

        static void LoadGame()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string relativePath = @"..\..\..\..\save\SaveText";
            string path = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

            string line;

            if (File.Exists(path))
            {

                using (StreamReader sr = new StreamReader(path))
                {
                    character.level = int.Parse(sr.ReadLine());
                    character.currentEXP = int.Parse(sr.ReadLine());
                    character.maxEXP = int.Parse(sr.ReadLine());
                    character.name = sr.ReadLine();
                    character.characterClass = sr.ReadLine();
                    character.attackDamage = int.Parse(sr.ReadLine());
                    character.extraAttackDamage = int.Parse(sr.ReadLine());
                    character.defense = int.Parse(sr.ReadLine());
                    character.extraDefense = int.Parse(sr.ReadLine());
                    character.health = int.Parse(sr.ReadLine());
                    character.gold = int.Parse(sr.ReadLine());

                    List<string> invenList = new List<string>();
                    List<Item> itemList = new List<Item>();

                    while ((line = sr.ReadLine()) != null)
                    {
                        invenList.Add(line);
                    }

                    for (int i = 0; i < storeItem.Count; i++)
                    {
                        if (invenList.Contains(storeItem[i].name))
                        {
                            itemList.Add(storeItem[i]);
                        }
                    }

                    character.list = itemList;
                }

            }

        }

        static void DeleteSaveFile()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string relativePath = @"..\..\..\..\save\SaveText";
            string textFile = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

            if (File.Exists(textFile))
            {
                File.Delete(textFile);
            }
        }

        public static void Main()
        {
            ItemSetting();
            LoadGame();
            HomeScene();
        }
    }
}
