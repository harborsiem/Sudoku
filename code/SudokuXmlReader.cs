using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Sudoku {

    public sealed class SudokuXmlReader {

        private SudokuXmlReader() { }

        public static void OpenSudokuFile(uint[,] inputs, string fileName) {
            try {
                FileInfo file = new FileInfo(fileName);
                if (file.Exists) {
                    XmlDocument doc = GetDocument(file);
                    LoadInputs(doc.DocumentElement, inputs);
                }
            }
            catch (Exception) {
                throw;
            }
        }

        private static XmlDocument GetDocument(FileInfo file) {
            XmlDocument document = new XmlDocument();
            try {
                document.Load(file.FullName);
            }
            catch (IOException) {
                throw;
            }
            return document;
        }

        /**
         * Read Inputs from xml file
         * 
         */
        private static void LoadInputs(XmlNode rootNode, uint[,] inputs) {
            try {
                if (rootNode.Name.Equals(SudokuXml.RootTag)) {
                    XmlNodeList itemList = rootNode.ChildNodes;
                    for (int i = 0; i < itemList.Count; i++) {
                        XmlNode item = itemList.Item(i);
                        if (item.Name.Equals(SudokuXml.ItemTag)) {
                            XmlNamedNodeMap itemAttributes = item.Attributes;

                            string row = itemAttributes.GetNamedItem(SudokuXml.RowAttribute).Value.Trim();
                            string column = itemAttributes.GetNamedItem(SudokuXml.ColumnAttribute).Value.Trim();
                            string value = itemAttributes.GetNamedItem(SudokuXml.ValueAttribute).Value.Trim();
                            int iRow = Int32.Parse(row);
                            int iColumn = Int32.Parse(column);
                            uint uValue = 0;
                            if (!String.IsNullOrEmpty(value)) {
                                uValue = UInt32.Parse(value);
                            }
                            if (iRow > 0 && iRow <= 9 && iColumn > 0 && iColumn <= 9 && uValue >= 0 && uValue <= 9) {
                                inputs[iRow - 1, iColumn - 1] = uValue;
                            } else {
                                throw new SudokuException("Wrong XML-Format");
                            }
                        } else {
                            throw new SudokuException("Wrong XML-Format");
                        }
                    }
                } else {
                    throw new SudokuException("Wrong XML-Format");
                }
            }
            catch (FormatException e) {
                throw new SudokuException("Wrong XML-Format", e);
            }
        }
    }
}
