﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FinalProject
{
    class Program
    {
        static Dictionary<int, AdventureRoom> myRooms;
        static List<AdventureVocab> myVocab;
        static Dictionary<int, AdventureItem> myItems;
        static Dictionary<int, string> myMessages;
        static AdventureActor player;

        static void Main(string[] args)
        {
            myRooms = new Dictionary<int, AdventureRoom>(140);
            myVocab = new List<AdventureVocab>();  //keys are not unique
            myItems = new Dictionary<int, AdventureItem>();
            myMessages = new Dictionary<int, string>();
            player = new AdventureActor();
            getData(@"..\..\..\rooms.txt");

            #region GAME STARTER
            Console.WriteLine(myMessages[1]);
            Console.WriteLine('\n');
            Console.WriteLine(myMessages[65]);
            string poss = Console.ReadLine();
            if (poss.ToUpper() == "YES" || poss.ToUpper() == "Y")
                Console.WriteLine(myMessages[142]);
            #endregion

            player.CurrentRoom = myRooms[1];
            string command;
            while (true)
            {
                #region HAS LIGHT
                if (player.CurrentRoom.Flags.HasFlag(RoomFlags.light) 
                    || ((player.HasItem(2) || player.CurrentRoom.HasItem(2)) 
                    && myItems[2].State == 1))
                {
                    Console.WriteLine(player.CurrentRoom);
                }
                else
                {
                    Console.WriteLine(myMessages[16]);
                }
                #endregion
                #region INPUT
                Console.Write(">");
                command = Console.ReadLine();
                string[] cmdlist = command.Split(' ');

                int myToken = Tokenize(cmdlist[0]);
                int myToken2 = 0;
                if (cmdlist.Length == 2)
                    myToken2 = Tokenize(cmdlist[1]);
                #endregion
                if (myToken == -1)  // Not a Vocab Word
                {
                    Console.WriteLine("I DON'T KNOW THAT WORD.");
                    continue;
                }
                #region VERBS
                else if (myToken > 2000 && myToken < 3000)  // verbs
                {
                    #region GET ALL
                    if (myToken == 2001 && cmdlist[1].ToUpper() == "ALL")   // special case - GET ALL
                    {
                        for (int i = 1; i <= 64; i++)
                        {
                            if (player.CurrentRoom.HasItem(i))
                            {
                                player.CurrentRoom.RemoveItem(i);
                                player.AddItem(myItems[i]);
                            }
                        }
                        Console.WriteLine("GOT {0}\n", cmdlist[1].ToUpper());
                        foreach (AdventureItem i in player.MyItems)
                        {
                            Console.WriteLine(i.ShortDescription);
                        }
                        Console.WriteLine('\n');
                    }
                    #endregion
                    #region SAY
                    else if (myToken == 2003)   // say or talk
                    {
                        for (int i = 1; i < cmdlist.Length; i++)
                        {
                            Console.Write("{0} ", cmdlist[i].ToUpper());
                        }
                        Console.WriteLine("\n");
                    }
                    #endregion
                    #region NON-VOCAB
                    else if (myToken2 == -1) // word that isnt in vocab
                    {
                        Console.WriteLine("CAN'T FIND {0}\n", cmdlist[1].ToUpper());
                    }
                    #endregion
                    #region GET
                    else if (myToken == 2001 && myToken2 > 1000 && myToken2 < 2000)  // get stuff
                    {
                        int itemNumber = myToken2 % 1000;
                        AdventureItem item = myItems[itemNumber];
                        
                        if (player.CurrentRoom.HasItem(8))  // getting the bird
                        {
                            if (!player.HasItem(4)) // cage
                            {
                                Console.WriteLine(myMessages[18]);
                            }
                            else if (player.HasItem(5) || player.HasItem(6)) // need to drop rod
                            {
                                Console.WriteLine(myMessages[19]);
                            }
                            else
                            {
                                player.CurrentRoom.RemoveItem(itemNumber);  // get bird
                                player.AddItem(item);
                                Console.WriteLine("GOT {0}\n", cmdlist[1].ToUpper());
                            }
                        }
                        else if (player.CurrentRoom.HasItem(itemNumber))
                        {
                            player.CurrentRoom.RemoveItem(itemNumber);
                            player.AddItem(item);
                            Console.WriteLine("GOT {0}\n", cmdlist[1].ToUpper());
                        }
                        else
                        {
                            Console.WriteLine("CAN'T FIND {0}\n", cmdlist[1].ToUpper());
                        }
                    }
                    #endregion
                    #region DROP
                    else if (myToken == 2002 && myToken2 > 1000 && myToken2 < 2000)     // drop items
                    {
                        int itemNumber = myToken2 % 1000;
                        AdventureItem item = myItems[itemNumber];
                       
                        if (player.HasItem(itemNumber))
                        {
                            player.RemoveItem(itemNumber);
                            player.CurrentRoom.AddItem(item);
                            Console.WriteLine("DROPPED {0}\n", cmdlist[1].ToUpper());
                        }
                        if (player.CurrentRoom.HasItem(8) && player.CurrentRoom.HasItem(11))
                        {
                            Console.WriteLine(myMessages[101]);
                            player.CurrentRoom.RemoveItem(8);
                            player.RemoveItem(8);
                        }
                        else
                        {
                            Console.WriteLine("CAN'T FIND {0}\n", cmdlist[1].ToUpper());
                        }

                    }
                    #endregion
                    #region UNLOCK
                    else if (myToken == 2004) //unlock
                    {
                        if (player.HasItem(1) && myItems[3].State == 0 && player.CurrentRoom.HasItem(3))
                        {
                            Console.WriteLine(myMessages[36]);
                            myItems[3].State++;
                        }
                        else if (!player.HasItem(1))
                        {
                            Console.WriteLine(myMessages[31]);
                        }
                        else if (myToken2 != 1055 || myToken2 != 1003)
                        {
                            Console.WriteLine(myMessages[33]);
                        }
                    }
                    #endregion
                    #region LOCK
                    else if (myToken == 2006)   // close or lock
                    {
                        if (player.HasItem(1) && myItems[3].State == 1 && player.CurrentRoom.HasItem(3))
                        {
                            Console.WriteLine(myMessages[35]);
                            myItems[3].State--;
                        }
                        else if (myToken2 != 1055 || myToken2 != 1003)
                        {
                            Console.WriteLine(myMessages[33]);
                        }

                    }
                    #endregion
                    #region LIGHT
                    else if (myToken == 2007)   //light
                    {
                        if (myToken2 == 1002)   // the lamp
                        {
                            if (player.HasItem(2) || player.CurrentRoom.HasItem(2))
                            {
                                myItems[2].State = 1;
                                Console.WriteLine(myMessages[39]);
                            }
                        }
                    }
                    #endregion
                    #region EXTINGUISH
                    else if (myToken == 2008)   // extinguish
                    {
                        if (myToken2 == 1002)   // the lamp
                        {
                            if (player.HasItem(2) || player.CurrentRoom.HasItem(2))
                            {
                                myItems[2].State = 0;
                                Console.WriteLine(myMessages[40]);
                                if (!player.CurrentRoom.Flags.HasFlag(RoomFlags.light))
                                {
                                    Console.WriteLine(myMessages[16]);
                                }
                            }
                        }
                    }
                    #endregion
                    #region WAVE
                    else if (myToken == 2009)     // wave or shake
                    {
                        if (myToken2 == 1005 || myToken2 == 1006)       // can only wave rod in specific room
                        {
                            if ((player.HasItem(5) || player.CurrentRoom.HasItem(5)) && myItems[12].State == 0)
                            {
                                myItems[12].State++;
                            }
                            else if ((player.HasItem(5) || player.CurrentRoom.HasItem(5)) && myItems[12].State == 1)
                            {
                                myItems[12].State++;
                            }
                            else if ((player.HasItem(5) || player.CurrentRoom.HasItem(5)) && myItems[12].State == 2)
                            {
                                myItems[12].State--;
                            }
                            else if((!player.HasItem(5) || !player.CurrentRoom.HasItem(5)) && myItems[12].State == 0)
                            {
                                Console.WriteLine(myMessages[76]);
                            }
                        }
                    }
                    #endregion
                    #region WALK
                    else if (myToken == 2011)   //walk
                    {
                        int walkToken = Tokenize(cmdlist[1]);
                        if (walkToken < 1000)
                        {
                            if (!player.CurrentRoom.HasItem(11))    // cant move by snake
                            {
                                Movement(myToken);  // created method so can be call from walk command
                            }
                            else
                            {
                                Console.WriteLine(myRooms[32]);
                            }
                        }
                    }
                    #endregion
                    #region POUR 
                    else if (myToken == 2013)   //POUR
                    {
                        if (myToken2 == 1020 || myToken2 == 1021)   //bottle or water
                        {
                            if (player.HasItem(20) && myItems[20].State == 0)
                            {
                                Console.WriteLine(myMessages[77]);
                                myItems[20].State++;
                            }
                            else if (player.HasItem(20) && myItems[20].State == 1)
                            {
                                Console.WriteLine("THERE IS NOTHING IN THE BOTTLE TO POUR!!!");
                            }
                            else
                            {
                                Console.WriteLine("YOU DO NOT HAVE {0}.", cmdlist[1].ToUpper());
                            }
                        }
                        else
                        {
                            Console.WriteLine("YOU MAY NOT POUR {0}.", cmdlist[1].ToUpper());
                        }
                    }
                    #endregion
                    #region EAT
                    else if (myToken == 2014)  //Eat
                    {
                        if (myToken2 == 1019) 
                        {
                            if (player.HasItem(19))
                            {
                                Console.WriteLine(myMessages[72]);
                                player.RemoveItem(19);
                            }
                            else if (!player.HasItem(19))
                            {
                                Console.WriteLine("YOU HAVE NOTHING TO EAT!!");
                            }
                        }
                        else if (myToken2 != 1019)
                        {
                            Console.WriteLine(myMessages[25]);
                        }
                    }
                    #endregion
                    #region DRINK
                    else if (myToken == 2015) //Drink
                    {
                        if (myToken2 == 1020 || myToken2 == 1021)   //bottle or water
                        {
                            if (player.HasItem(20) && myItems[20].State == 0)
                            {
                                Console.WriteLine(myMessages[74]);
                                myItems[20].State++;
                            }
                            else if (player.HasItem(20) && myItems[20].State == 1)
                            {
                                Console.WriteLine("THERE IS NOTHING IN THE BOTTLE TO DRINK!!!");
                            }
                            else if (player.CurrentRoom.Flags.HasFlag(RoomFlags.liquid))
                            {
                                Console.WriteLine(myMessages[73]);
                            }
                            else
                            {
                                Console.WriteLine(myMessages[110]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("ARE YOU SURE YOU WANT TO DRINK {0}", cmdlist[1].ToUpper());
                            string drink = Console.ReadLine();
                            if (drink.ToUpper() == "YES" || drink.ToUpper() == "Y")
                            {
                                Console.WriteLine(myMessages[81]);
                                drink = Console.ReadLine();
                                if (drink.ToUpper() == "YES" || drink.ToUpper() == "Y")
                                {
                                    Console.WriteLine(myMessages[82]);
                                }
                            }
                        }
                    }
                    #endregion
                    #region QUIT
                    else if (myToken == 2018) //quit
                    {
                        Console.WriteLine(myMessages[143]);
                        string ending = Console.ReadLine();

                        if (ending.ToUpper() == "YES" || ending.ToUpper() == "Y")
                        {
                            Console.WriteLine("\nADVENTURING IS OBVIOUSLY NOT FOR YOU!\n");
                            break;
                        }
                        else if (ending.ToUpper() == "NO" || ending.ToUpper() == "N")
                        {
                            Console.WriteLine("\n");
                            continue;
                        }
                        else
                        {
                            Console.WriteLine(myMessages[12]);
                            Console.WriteLine("\n");
                        }
                    }
                    #endregion
                    #region FIND
                    else if (myToken == 2019)
                    {
                        Console.WriteLine(myMessages[15]);
                    }
                    #endregion
                    #region INVENTORY
                    else if (myToken == 2020)    // inventory
                    {
                        foreach (AdventureItem i in player.MyItems)
                        {
                            Console.WriteLine(i.ShortDescription);
                        }
                        Console.WriteLine('\n');
                    }
                    #endregion
                    #region FILL
                    else if (myToken == 2022)   // fill
                    {
                        if (myToken2 == 1020 || myToken2 == 1021)   //bottle or water
                        {
                            if (player.HasItem(20) && myItems[20].State == 1 && player.CurrentRoom.Flags.HasFlag(RoomFlags.liquid))
                            {
                                Console.WriteLine(myMessages[107]);
                                myItems[20].State = 0;
                            }
                            else if (player.HasItem(20) && myItems[20].State == 1 && player.CurrentRoom.Flags.HasFlag(RoomFlags.OilWater))
                            {
                                Console.WriteLine(myMessages[108]);
                                myItems[20].State = 0;
                            }
                            else if (player.HasItem(20) && myItems[20].State == 0)
                            {
                                Console.WriteLine(myMessages[105]);
                            }
                            else
                            {
                                Console.WriteLine(myMessages[109]);
                            }
                        }
                    }
                    #endregion
                    #region THROW
                    else if (myToken == 2017)
                    {
                        if (myToken2 == 1008 && player.CurrentRoom.HasItem(11))
                        {
                            player.CurrentRoom.RemoveItem(11); //remove snake from room
                            player.RemoveItem(8); //remove bird from inven
                            player.CurrentRoom.AddItem(myItems[8]); //add bird to room 
                            Console.WriteLine(myMessages[30]); 
                        }
                        else
                        {
                            int itemNumber = myToken2 % 1000;
                            AdventureItem item = myItems[itemNumber];

                            if (player.HasItem(itemNumber))
                            {
                                player.RemoveItem(itemNumber);
                                player.CurrentRoom.AddItem(item);
                                Console.WriteLine("DROPPED {0}\n", cmdlist[1].ToUpper());
                            }
                            else
                            {
                                Console.WriteLine("CAN'T FIND {0}\n", cmdlist[1].ToUpper());
                            }
                        }
                    }
                    #endregion
                    #region CURSE WORDS

                    else if (myToken == 2032)
                    {
                        Console.WriteLine(myMessages[79]);
                    }
                    #endregion
                }
                #endregion
                #region MOVEMENT
                else if (myToken < 1000)    // movement through map
                {
                    if (player.CurrentRoom.HasItem(11)) // cant move by snake
                    {
                        Console.WriteLine(myRooms[32]);  // cant move by snake
                    }
                    else if (player.CurrentRoom.HasItem(12) && myItems[12].State != 1)  // if bride is not there
                    {
                        Console.WriteLine(myMessages[97]);  //cant move across
                    }                                       //would be able to cross but something wrong with movement table
                    #region XYZZY
                    else if (myToken == 62)
                    {
                        if (player.CurrentRoom != myRooms[11])
                        {
                            player.CurrentRoom = myRooms[11];
                        }
                        else if (player.CurrentRoom == myRooms[11])
                        {
                            player.CurrentRoom = myRooms[1];
                        }
                    }
                    #endregion
                    #region Y2
                    else if (myToken == 55)
                    {
                        if (player.CurrentRoom != myRooms[33])
                        {
                            player.CurrentRoom = myRooms[33];
                        }
                        else if (player.CurrentRoom == myRooms[33])
                        {
                            player.CurrentRoom = myRooms[1];
                        }
                    }
                    #endregion
                    else
                    {
                        Movement(myToken);  // created method so can be call from walk command
                    }
                }
                #endregion
            }
        }

        static void Movement(int q)
        {
            foreach (AdventureExit e in player.CurrentRoom.Exits)
            {
                if (e.Vocab.Contains(q))
                {
                    if (e.Conditional == 0 || e.Conditional == 100)
                        player.CurrentRoom = myRooms[e.Destination];
                    else if (e.Conditional >= 300)
                    {
                        int itemNumber = e.Conditional % 100;
                        int forbiddenState = (e.Conditional / 100) - 3;
                        if (myItems[itemNumber].State != forbiddenState)
                            player.CurrentRoom = myRooms[e.ComputedDest];
                    }

                    else if (e.Conditional > 200 && e.Conditional <= 300)
                    {
                        if (player.HasItem(e.Conditional - 200) || player.CurrentRoom.HasItem(e.Conditional - 200))
                        {
                            player.CurrentRoom = myRooms[e.ComputedDest];
                        }
                        else
                        {
                            Console.WriteLine("YOU OR YOUR ROOM DO NOT HAVE THE NECESSARY ITEMS.");
                        }
                    }

                    else if (e.Conditional > 100 && e.Conditional <= 200)
                    {
                        int itemNumber = e.Conditional - 100;
                        if (player.HasItem(itemNumber))
                        {
                            player.CurrentRoom = myRooms[e.ComputedDest];
                        }
                        else if (!player.HasItem(itemNumber))

                        {
                            player.CurrentRoom = myRooms[34];   //move to next room if you dont have gold
                        }
                        else
                        {
                            Console.WriteLine("YOU DON'T HAVE THE NECESSARY ITEM.");
                        }
                    }

                    else if (e.Conditional > 300 && e.Conditional <= 400)
                    {
                        if (player.CurrentRoom.HasItem(11))
                        {
                            player.CurrentRoom = myRooms[19];
                            Console.WriteLine(myMessages[20]);
                        }
                        else if ((e.Conditional % 100) != 0)
                        {
                            player.CurrentRoom = myRooms[e.ComputedDest];
                        }
                    }

                    else
                        throw new NotImplementedException("cant handle conditional movements");

                        break;
                }
            }
        }
        static public int Tokenize(string s)
        {
            foreach (AdventureVocab v in myVocab)
            {
                if (string.Compare(v.Word, 0, s.ToUpper(), 0, 5) == 0)
                {
                    return v.Index;
                }
            }
            return -1;
        }

        /*static void testSection4()
        {
            string input;
            int number;
            bool found;

            while(true)
            {
                input = Console.ReadLine();
                found = false;
                if(int.TryParse(input, out number))
                {
                    if (number == 0)
                        return;
                    //find num in list
                    foreach(AdventureVocab v in myVocab)
                    {
                        if(v.Index == number)
                        {
                            Console.WriteLine(v);
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                        Console.WriteLine("Not found.");
                }
                else
                {
                    //find word in list
                    foreach (AdventureVocab v in myVocab)
                    {
                        if (string.Compare(v.Word, 0, input.ToUpper(), 0, 5) == 0)
                        {
                            Console.WriteLine(v);
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                        Console.WriteLine("Not found.");
                }
            }
        }
        */

        #region GET SECTIONS
        static void GetSection1(StreamReader fs)
        {
            string tmp;
            fs.ReadLine(); //section 1
            //myRooms.Add(new AdventureRoom());
            //Console.WriteLine(fs.ReadLine());
            while (true)
            {
                // get room descriptions

                tmp = fs.ReadLine();
                string[] q = tmp.Split('\t');
                int roomNumber = int.Parse(q[0]);

                if (roomNumber < 0)
                    break;
                if (!myRooms.ContainsKey(roomNumber))
                    myRooms[roomNumber] = new AdventureRoom();

                if (myRooms[roomNumber].Description.Length == 0)
                    myRooms[roomNumber].Description = q[1];
                else
                    myRooms[roomNumber].Description += " " + q[1];
                // done with room descriptions
            }
        }
        static void GetSection2(StreamReader fs)
        {
            string tmp;
            tmp = fs.ReadLine(); //start section 2
            while (true)
            {
                tmp = fs.ReadLine();
                string[] q = tmp.Split('\t');
                int roomNumber = int.Parse(q[0]);

                if (roomNumber < 0)
                    break;
                myRooms[roomNumber].ShortDescription = q[1];
            }
        }
        static void GetSection3(StreamReader fs)
        {
            string tmp;
            fs.ReadLine();
            while (true)
            {
                tmp = fs.ReadLine();    // one exit
                string[] q = tmp.Split('\t');
                int roomNumber = int.Parse(q[0]);
                if (roomNumber < 0)
                    break;
                int destination = int.Parse(q[1]);
                AdventureExit x = new AdventureExit(roomNumber,destination);
                myRooms[x.Source].Exits.Add(x);
                for (int i=2; i < q.Length; i++)
                {
                    x.AddVocab(int.Parse(q[i]));
                }
                
                
            }
        }
        static void GetSection4(StreamReader fs)
        {
            string tmp; //section4
            fs.ReadLine();
            while (true)
            {
                tmp = fs.ReadLine();
                string[] q = tmp.Split('\t');
                int vocabNumber = int.Parse(q[0]);
                int M = vocabNumber / 1000;
                int index = M % 1000;
                if (vocabNumber < 0)
                    return;

                myVocab.Add(new AdventureVocab(vocabNumber, q[1]));
            }
        }
        static void GetSection5(StreamReader fs)
        {
            AdventureItem item = null;
            string tmp;
            int itemNumber;

            fs.ReadLine(); // section 5

            tmp = fs.ReadLine();
            string[] q = tmp.Split('\t');
            itemNumber = int.Parse(q[0]);

            while (itemNumber > 0)
            {
                if (item != null)
                    myItems[item.Index] = item;

                item = new AdventureItem();
                itemNumber = int.Parse(q[0]);
                item.Index = itemNumber;
                item.ShortDescription = q[1];

                // getting state description
                tmp = fs.ReadLine();
                q = tmp.Split('\t');
                int descNumber = int.Parse(q[0]);
                int t = descNumber % 100;

                while (t == 0)  //description
                {
                    t = descNumber / 100;
                    if (item.StateDescriptions.Count <= t)
                    {
                        item.StateDescriptions.Add(q[1]);
                    }
                    else
                    {
                        item.StateDescriptions[t] += " " + q[1];
                    }

                    tmp = fs.ReadLine();
                    q = tmp.Split('\t');
                    descNumber = int.Parse(q[0]);
                    t = descNumber % 100;
                }

                itemNumber = descNumber;
                // q and itemNumber must contain next data
            }
        }
        static void GetSection6(StreamReader fs)
        {
            string tmp;
            fs.ReadLine();
            while (true)
            {
                tmp = fs.ReadLine();    // one exit
                string[] q = tmp.Split('\t');
                int roomNumber = int.Parse(q[0]);
                if (roomNumber < 0)
                    break;

                if (myMessages.ContainsKey(roomNumber))
                    myMessages[roomNumber] += " " + q[1];
                else
                    myMessages[roomNumber] = q[1];
            }
        }
        static void GetSection7(StreamReader fs)
        {
            string tmp;     // section 7
            fs.ReadLine();
            while (true)
            {
                tmp = fs.ReadLine();    // one exit

                string[] q = tmp.Split('\t');
                int itemNumber = int.Parse(q[0]);
                if (itemNumber < 0)
                    break;

                if (!myItems.ContainsKey(itemNumber))
                {
                    continue;
                }

                if (q.Length == 2)
                {
                    int room = int.Parse(q[1]);

                    if (room != 0)
                        myRooms[room].AddItem(myItems[itemNumber]);
                }

                else if (q.Length == 3)
                {
                    int room1 = int.Parse(q[1]);
                    int room2 = int.Parse(q[2]);
                    myItems[itemNumber].Immovable = true;

                    if (room1 != 0)
                        myRooms[room1].AddItem(myItems[itemNumber]);

                    if (room2 != -1 && room2 != 0) 
                    {
                        myRooms[room2].AddItem(myItems[itemNumber]);
                    }
                   
                }
                

            }
        }
        static void GetSection9(StreamReader fs)
        {
            fs.ReadLine(); //Section 9
            string tmp;

            tmp = fs.ReadLine(); //one item

            string[] q = tmp.Split('\t');
            int bitNumber = int.Parse(q[0]);

            while (bitNumber >= 0)
            {
                for (int i = 1; i < q.Length; i++)
                {
                    int t = int.Parse(q[i]);
                    myRooms[t].Flags |= (RoomFlags)(1 << bitNumber);
                }
                tmp = fs.ReadLine();
                q = tmp.Split('\t');
                bitNumber = int.Parse(q[0]);
            }
        }
        static void SkipSection(StreamReader fs)
        {
            fs.ReadLine();
            string tmp;
            while(true)
            {
                tmp = fs.ReadLine();

                string[] q = tmp.Split('\t');
                int itemNumber = int.Parse(q[0]);
                if (itemNumber < 0)
                    break;
                //skip
            }
        }
        #endregion

        static void getData(string filename)
        {
            using (StreamReader fs = new StreamReader(filename))
            {
                GetSection1(fs);
                GetSection2(fs);
                GetSection3(fs);
                GetSection4(fs);
                GetSection5(fs);
                GetSection6(fs);
                GetSection7(fs);
                SkipSection(fs);
                GetSection9(fs);

                //Console.WriteLine(fs.ReadLine());
            }
        }
    }
}