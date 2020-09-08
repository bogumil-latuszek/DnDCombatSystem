using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace DnDBattleSystem
{
    public class DmgEffects
    {
        public string effectName;
        public bool effect;
        public int effectChance;
        public int effectDmg;
        public int effectTime;
    }
    public class Attacks
    {

        public Attacks()
        {
            this.FillEffects();
        }
        public DmgEffects[] dmgEff = new DmgEffects[10];
        protected virtual void FillEffects()
        {
            this.dmgEff[0].effectName = "poison";
            this.dmgEff[0].effect = false;
            this.dmgEff[0].effectChance = 40;
            this.dmgEff[0].effectDmg = 2;
            this.dmgEff[0].effectTime = 5;

            this.dmgEff[1].effectName = "burn";
            this.dmgEff[1].effect = false;
            this.dmgEff[1].effectChance = 50;
            this.dmgEff[1].effectDmg = 5;
            this.dmgEff[1].effectTime = 2;

            this.dmgEff[2].effectName = "bleed";
            this.dmgEff[2].effect = false;
            this.dmgEff[2].effectChance = 20;
            this.dmgEff[2].effectDmg = 3;
            this.dmgEff[2].effectTime = 3;
        }
        public enum attackRange
        {
            singleTarget,
            twoTargets,
            threeTargets,
            AOE
        }
        public enum attackElementalType
        {
            Fire,
            Dark,
            Cold,
            Chemical,
            Magic,
            Blunt,
            Cut,
            Pierce,

        }
        public int dmgModyfier;
        public int hitChance;
        public int numberOfTargets;
        public string attackName;

    }
    public class Creature
    {
        public string Name;
        public static Random RandomGen = new Random();
        public int HPValue;
        public int attackPower;
        public bool ableToAttack;
        public int dodgeChance;
        public class EffectsOnMe
        {
            public bool effect;
            public string effectName;
            public int effectDamage;
            public int effectTime;
        }
        List<EffectsOnMe> EffectsOnMeList = new List<EffectsOnMe>();
        public void StatusEffectsUpdate()
        {
            for (int i = 0; i < this.EffectsOnMeList.Count; i++)
            {
                
                if (this.EffectsOnMeList[i].effect)
                {
                    this.HPValue -= this.EffectsOnMeList[i].effectDamage;
                    this.EffectsOnMeList[i].effectTime -= 1;//wtf ten error?
                    if(this.EffectsOnMeList[i].effectTime <= 0)
                    {
                        this.EffectsOnMeList[i].effect = false;
                    }
                }

            }
        }

        List<Attacks> AttacksList = new List<Attacks>();

        public void StatusEffectsDealt(Attacks myAttack, Creature myEnemy)
        {
            int randomEffectChance = RandomGen.Next(101);
            for (int i = 0; i< myAttack.dmgEff.Length; i++)
            {
                if(myAttack.dmgEff[i].effect == true && myAttack.dmgEff[i].effectChance >= randomEffectChance)
                {
                    for(int a = 0; a< myEnemy.EffectsOnMeList.Count; a++)
                    { 
                        if(myEnemy.EffectsOnMeList[a].effectName == myAttack.dmgEff[i].effectName)
                        {
                            myEnemy.EffectsOnMeList[a].effectDamage += myAttack.dmgEff[i].effectDmg;
                            myEnemy.EffectsOnMeList[a].effectTime += 1;
                        }
                        else
                        {
                            myEnemy.EffectsOnMeList.Add(new EffectsOnMe {effectName= myAttack.dmgEff[i].effectName, effect = true, effectDamage = myAttack.dmgEff[i].effectDmg, effectTime = myAttack.dmgEff[i].effectTime  });
                            Console.WriteLine(" new effect added");
                        }
                    } //tutaj chcemy wpisać efekty z enemy attack do listy efektów na this
                }
            }
        }
        public void DeathCheck(Creature myEnemy )
        {
            if (myEnemy.HPValue <= 0)
            {
                Console.WriteLine(myEnemy.Name + "died");
                myEnemy.ableToAttack = false;
            }
        }
        public int ChooseEnemy(List<Creature> AvailableEnemies)
        {
            int selection = RandomGen.Next(AvailableEnemies.Count + 1);
            return selection;
        }
        virtual public void CommenceFight() 
        {
            int attackNumber = RandomGen.Next(this.AttacksList.Count);
            int attackTargets = AttacksList[attackNumber].numberOfTargets;
            if (attackTargets > MyEnemieslist.Count)
            {
                attackTargets = MyEnemieslist.Count;
            }
            List<Creature> remainingEnemies = MyEnemieslist;
            for (int enemyNumber = 0; enemyNumber < attackTargets; enemyNumber++)
            {
                int selectedEnemyIdx = ChooseEnemy(remainingEnemies);
                Creature enemyTarget = remainingEnemies[selectedEnemyIdx];
                remainingEnemies.RemoveAt(selectedEnemyIdx);

                int hit = RandomGen.Next(101);
                if (this.AttacksList[attackNumber].hitChance >= hit)
                {
                    /*int dodge = RandomGen.Next(101);
                    if (dodge > enemyTarget.dodgeChance)*/
                    if (UnsuccessfulDodge(enemyTarget))
                    {
                        int trueDamage = this.AttacksList[attackNumber].dmgModyfier * this.attackPower;//tu się przyda zaokrąglanie
                        enemyTarget.HPValue -= trueDamage;
                        Console.WriteLine(enemyTarget.Name + "got hit for" + trueDamage);
                        StatusEffectsDealt(this.AttacksList[attackNumber], enemyTarget);
                    }
                    else
                    {
                        Console.WriteLine(enemyTarget.Name + "dodged the"); //here goes the attack name
                    }
                }
                else
                {
                    Console.WriteLine(this.Name + "missed its target");
                }
            }
        }

        public bool UnsuccessfulDodge(Creature enemyTarget)
        {
            int dodge = RandomGen.Next(101);
            return (dodge > enemyTarget.dodgeChance);
        }
        public List<Creature> MyEnemieslist = new List<Creature>();
        virtual public void AttackTurn(List<Creature> PlayersOnBattlefield)
        {
            this.MyEnemieslist = PlayersOnBattlefield;
            StatusEffectsUpdate();

            if (this.HPValue <= 0)
            {
                Console.Write("this enemy is dead");
                ableToAttack = false;
            }
            if (ableToAttack == true) 
            {
                CommenceFight();
            }
        }

        public Creature(int hpValue = 100)
        {
            this.HPValue = hpValue;
            this.ableToAttack = true;
        }
        public void Descript()
        {

        }


    }
    class Manticore : Creature
    {
        public Manticore(int hpValue = 200)
        {
            this.HPValue = hpValue;
        }
        PoisonedSwordSlash slash1 = new PoisonedSwordSlash();
    }
    class Player : Creature
    {
        public string Name;
        public static Random RandomGen = new Random();
        public int HPValue;
        public int attackPower;
        public bool ableToAttack;
        public int dodgeChance;
        List<EffectsOnMe> EffectsOnMeList = new List<EffectsOnMe>();
        List<Attacks> AttacksList = new List<Attacks>();
        List<Creature> MyEnemieslist = new List<Creature>();
        public Player() : base()
        {

        }
        public Attacks GetAttack(List<Attacks> AvailableAttacks)
        {
            do
            {
                Console.WriteLine("Select attack:");
                for (int i = 0; i < AvailableAttacks.Count; i++)
                {
                    Console.WriteLine((i + 1) + " - " + AvailableAttacks[i].attackName);
                }
                int userSelection = Int32.Parse(Console.ReadLine());
                if ((userSelection >= 1) && (userSelection <= AvailableAttacks.Count))
                {
                    return AvailableAttacks[userSelection - 1];
                }
                else
                {
                    Console.WriteLine("Wrong number selected - try again");
                }
            } while (true););
        }
        public int ChooseEnemy(List<Creature> AvailableEnemies)
        {
            do
            {
                Console.WriteLine("Select attack target:");
                for (int i = 0; i < AvailableEnemies.Count; i++)
                {
                    Console.WriteLine((i+1) + " - " + AvailableEnemies[i].Name);
                }
                int userSelection = Int32.Parse(Console.ReadLine());
                if ((userSelection >= 1) && (userSelection <= AvailableEnemies.Count))
                {
                    return userSelection - 1;
                }
                else
                {
                    Console.WriteLine("Wrong number selected - try again");
                }
            } while (true);
        }
        public void CommenceFight() //this is similar to CoommenceFight() in base class
        {
            Console.WriteLine("Write attack name:");
            string attackName = Console.ReadLine(); //will this give us only attack name?
            Attacks myAttack  = GetAttack(AttacksList);
            int attackTargets = myAttack.numberOfTargets;
            if (attackTargets > MyEnemieslist.Count)
            {
                attackTargets = MyEnemieslist.Count;
            }
            List<Creature> remainingEnemies = MyEnemieslist;
            for (int enemyNumber = 0; enemyNumber < attackTargets; enemyNumber++)
            {
                int selectedEnemyIdx = ChooseEnemy(remainingEnemies);
                Creature enemyTarget = remainingEnemies[selectedEnemyIdx];
                remainingEnemies.RemoveAt(selectedEnemyIdx);
                int hit = RandomGen.Next(101);
                if (myAttack.hitChance >= hit)
                {
                    /*int dodge = RandomGen.Next(101);
                    if (dodge > enemyTarget.dodgeChance)*/
                    if (UnsuccessfulDodge(enemyTarget))
                    {
                        int trueDamage = myAttack.dmgModyfier * this.attackPower;//tu się przyda zaokrąglanie
                        enemyTarget.HPValue -= trueDamage;
                        Console.WriteLine(enemyTarget.Name + "got hit for" + trueDamage);
                        StatusEffectsDealt(myAttack, enemyTarget);
                    }
                    else
                    {
                        Console.WriteLine(enemyTarget.Name + "dodged the" + myAttack.attackName); //here goes the attack name
                    }
                }
                else
                {
                    Console.WriteLine(myAttack.attackName + "missed" + enemyTarget.Name);
                }
            }

        }
    }

    class PoisonedSwordSlash : Attacks
    {
        attackElementalType myType = attackElementalType.Cut;
        attackRange myRange= attackRange.singleTarget;
        int hitChance = 65;
        public PoisonedSwordSlash( ) :base()                     
        {
            dmgEff[0].effect = true;
        }
    }
    class Program
    {
        public static void AddPlayerToScene()
        {
            Console.WriteLine("");
        }
        public static void AddPlayerToBattlefield()
        {

        }
        public static void CheckIfBattleIsPossible()
        {
            if()
        }
        public static List<Player> PlayersOnScene = new List<Player>();
        public static List<Player> PlayersOnBattlefield = new List<Player>();
        public static List<Creature> EveryoneOnBattlefield = new List<Creature>();
        static void Main(string[] args)
        {

            bool preparations = true;
            do
            {
                Console.WriteLine("1) add player to the library 2)Add a creature to the battlefield 3) go to battle");
                int userImput = Int32.Parse(Console.ReadLine());
                switch (userImput)
                {
                    case 1:
                        Console.WriteLine("");
                        AddPlayerToScene();
                        break;
                    case 2:
                        AddPlayerToBattlefield();
                        break;
                    case 3:
                        CheckIfBattleIsPossible();
                        preparations = false;
                        break;
                    default:
                        Console.WriteLine("unknown option");
                        break;
                }
            } while (preparations == true);
            for (int i= 0; i < EveryoneOnBattlefield.Count; i++)
            {
                /*if (EnemiesOnBattlefield[i].GetType == Player)
                {
                    Console.WriteLine("Attack Name:");
                    string attackName = Console.ReadLine();

                }
                else
                {
                    EnemiesOnBattlefield[i].MyEnemieslist = PlayersOnBattlefield;
                    EnemiesOnBattlefield[i].AttackTurn(PlayersOnBattlefield);
                }*/
                EveryoneOnBattlefield[i].AttackTurn(PlayersOnBattlefield);
            }
        }
    }
}
