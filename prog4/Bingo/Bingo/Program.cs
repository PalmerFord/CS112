﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Linq;

namespace Bingo
{
    class Program
    {
        private static RelationshipGraph rg;

        // Read RelationshipGraph whose filename is passed in as a parameter.
        // Build a RelationshipGraph in RelationshipGraph rg
        private static void ReadRelationshipGraph(string filename)
        {
            rg = new RelationshipGraph();                           // create a new RelationshipGraph object

            string name = "";                                       // name of person currently being read
            int numPeople = 0;
            string[] values;
            Console.Write("Reading file " + filename + "\n");
            try
            {
                string input = System.IO.File.ReadAllText(filename);// read file
                input = input.Replace("\r", ";");                   // get rid of nasty carriage returns 
                input = input.Replace("\n", ";");                   // get rid of nasty new lines
                string[] inputItems = Regex.Split(input, @";\s*");  // parse out the relationships (separated by ;)
                foreach (string item in inputItems) 
		{
                    if (item.Length > 2)                            // don't bother with empty relationships
                    {
                        values = Regex.Split(item, @"\s*:\s*");     // parse out relationship:name
                        if (values[0] == "name")                    // name:[personname] indicates start of new person
                        {
                            name = values[1];                       // remember name for future relationships
                            rg.AddNode(name);                       // create the node
                            numPeople++;
                        }
                        else
                        {               
                            rg.AddEdge(name, values[1], values[0]); // add relationship (name1, name2, relationship)

                            // handle symmetric relationships -- add the other way
                            if (values[0] == "hasSpouse" || values[0] == "hasFriend")
                                rg.AddEdge(values[1], name, values[0]);

                            // for parent relationships add child as well
                            else if (values[0] == "hasParent")
                                rg.AddEdge(values[1], name, "hasChild");
                            else if (values[0] == "hasChild")
                                rg.AddEdge(values[1], name, "hasParent");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read file {0}: {1}\n", filename, e.ToString());
            }
            Console.WriteLine(numPeople + " people read");
        }

        // Show the relationships a person is involved in
        private static void ShowPerson(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
                Console.Write(n.ToString());
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's friends
        private static void ShowFriends(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s friends: ",name);
                List<GraphEdge> friendEdges = n.GetEdges("hasFriend");
                foreach (GraphEdge e in friendEdges) {
                    Console.Write("{0} ",e.To());
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        //Show a person's siblings
        private static void ShowSiblings(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s siblings: ", name);
                List<GraphEdge> parentEdges = n.GetEdges("hasParent");
                int i = 0;
                foreach (GraphEdge e in parentEdges)
                {
                    if (i == 0)
                    {
                        GraphNode m = rg.GetNode(e.To());
                        List<GraphEdge> childEdges = m.GetEdges("hasChild");
                        foreach (GraphEdge j in childEdges)
                        {
                            if (j.To() != name)
                            {
                                Console.Write("{0} ", j.To());
                            }
                        }
                    }
                    i++;
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        private static void ShowDescendants(string name, int depth = 0)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                List<GraphEdge> childEdges = n.GetEdges("hasChild");
                if (depth == 0)
                {
                    foreach (GraphEdge e in childEdges)
                    {
                        Console.Write("child: {0} ", e.To());
                        Console.WriteLine();
                        ShowDescendants(e.To(), (depth + 1));
                    }
                }
                else
                {
                    foreach (GraphEdge e in childEdges)
                    {
                       for (int i = depth; i > 1; i--)
                        {
                            Console.Write("great ");
                        }
                        Console.Write("grandchild: {0} ", e.To());
                        Console.WriteLine();
                        ShowDescendants(e.To(), (depth + 1));
                    }
                }
            }
            else
                Console.WriteLine("{0} not found", name);
        }

            // accept, parse, and execute user commands
            private static void CommandLoop()
        {
            string command = "";
            string[] commandWords;
            Console.Write("Welcome to Harry's Dutch Bingo Parlor!\n");

            while (command != "exit")
            {
                Console.Write("\nEnter a command: ");
                command = Console.ReadLine();
                commandWords = Regex.Split(command, @"\s+");        // split input into array of words
                command = commandWords[0];

                if (command == "exit")
                    ;                                               // do nothing

                // read a relationship graph from a file
                else if (command == "read" && commandWords.Length > 1)
                    ReadRelationshipGraph(commandWords[1]);

                // show information for one person
                else if (command == "show" && commandWords.Length > 1)
                    ShowPerson(commandWords[1]);

                else if (command == "friends" && commandWords.Length > 1)
                    ShowFriends(commandWords[1]);

                else if (command == "siblings" && commandWords.Length > 1)
                    ShowSiblings(commandWords[1]);

                else if (command == "descendants" && commandWords.Length > 1)
                    ShowDescendants(commandWords[1]);

                else if (command == "orphans")
                    rg.GetOrphans();

                // dump command prints out the graph
                else if (command == "dump")
                    rg.Dump();

                // illegal command
                else
                    Console.Write("\nLegal commands: read [filename], dump, siblings [personname, descendants, show [personname],\n  friends [personname] Orphans, exit\n");
            }
        }

        static void Main(string[] args)
        {
            CommandLoop();
        }
    }
}
