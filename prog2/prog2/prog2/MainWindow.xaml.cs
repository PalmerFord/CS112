﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;

namespace BabbleSample
{
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        Dictionary<string, LinkedList<string>> hashTable = new Dictionary<string, LinkedList<string>>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".txt"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                words = Regex.Split(input, @"\s+");       // split into array of words
            }
            analyzeInput(orderComboBox.SelectedIndex);
        }

        private void analyzeInput(int order)
        {
            if (order == 1 && words != null)
            {
                MessageBox.Show("Analyzing at order: " + order);
                string preWord = "";
                int i = 0;
                foreach (string word in words)
                {
                    if (!hashTable.ContainsKey(preWord))
                    { 
                        hashTable.Add(preWord, new LinkedList<string>());
                    }
                    hashTable[preWord].AddLast(word);
                    preWord = word;
                    if (i == words.Length - 1)
                    {
                        hashTable[word].AddLast("");
                    }
                    i++;
                }
            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "";
            string preWord = words[0];
            string word = null;
            textBlock1.Text += " " + preWord;
            for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
            {
                word = randomNext(preWord);
                textBlock1.Text += " " + word;
                preWord = word;
            }
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);
        }

        private string randomNext(string preWord)
        {
            Random rd = new Random();
            int rand_num = rd.Next(0, hashTable[preWord].Count);
            var currentNode = hashTable[preWord].First;
            for (int i = 0; i != rand_num; i++)
            { 
                currentNode = currentNode.Next;
            }
            return currentNode.Value;
        }
    }
}
