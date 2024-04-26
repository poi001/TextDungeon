using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDeongeon
{
    internal class Character
    {
        //레벨
        public int level = 1;
        //현재 경험치
        public int currentEXP = 0;
        //전체 경험치
        public int maxEXP = 1;
        //이름
        public string name = "Cristiano Ronaldo";
        //캐릭터 직업
        public string characterClass = "전사";
        //공격력
        public int attackDamage = 10;
        //추가 공격력
        public int extraAttackDamage = 0;
        //방어력
        public int defense = 5;
        //추가 방어력
        public int extraDefense = 0;
        //체력
        public int health = 100;
        //골드
        public int gold = 1500;

        //가지고 있는 장비
        public List<Item> list = new List<Item>();
        //장착중인 무기
        public Item weapon = new Item();
        //장착중인 갑옷
        public Item armor = new Item();

        public void EquipWeapon(ref Item equipWeapon)
        {
            this.weapon = equipWeapon;
            calAttackDamage();
        }

        public void EquipArmor(ref Item equipArmor)
        {
            this.armor = equipArmor;
            calDefense();
        }

        public void TakOffWeapon()
        {
            this.weapon = null;
            calAttackDamage();
        }

        public void TakOffArmor()
        {
            this.armor = null;
            calDefense();
        }

        private void calAttackDamage()
        {
            if(this.weapon != null)
            {
                extraAttackDamage = this.weapon.status;
                attackDamage += extraAttackDamage;
            }
            else
            {
                attackDamage -= extraAttackDamage;
                extraAttackDamage = 0;
            }

        }

        private void calDefense()
        {
            if (this.armor != null)
            {
                extraDefense = this.armor.status;
                defense += extraDefense;
            }
            else
            {
                defense -= extraDefense;
                extraDefense = 0;
            }
        }

        public void AddEXP()
        {
            currentEXP++;
            if (maxEXP <= currentEXP) LevelUp();
        }

        public void LevelUp()
        {
            level++;
            currentEXP = 0;
            maxEXP = level;

            this.defense += 1;
            this.attackDamage += 1;

            Console.WriteLine("======================LevelUp======================");
            Console.WriteLine("레벨업을 하셨습니다.");
            Console.WriteLine();
            Console.WriteLine("레벨업: {0} -> {1}", this.level - 1, this.level);
            Console.WriteLine("방어력 증가 + 1, 현재 방어력 : {0}", this.defense);
            Console.WriteLine("공격력 증가 + 1, 현재 공격력 : {0}", this.attackDamage);
            Console.WriteLine("최대 경험치 : {0}", maxEXP);
            Console.WriteLine();
            Console.WriteLine("===================================================");
        }
    }
}
