using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Sudoku {
    public sealed class SudokuXmlWriter {

        private SudokuXmlWriter() { }

        public static void SaveInputs(uint[,] inputs, string fileName) {
            FileInfo file = new FileInfo(fileName);

            XmlDocument doc = CreateDocument();
            SetInputs(inputs, doc);
            SaveDocument(doc, file);
        }

        public static XmlDocument CreateDocument() {
            XmlDocument document = new XmlDocument();
            //Create an XML declaration. 
            XmlDeclaration xmldecl;
            xmldecl = document.CreateXmlDeclaration("1.0", "UTF-8", "no");
            xmldecl.Encoding = "UTF-8";
            document.AppendChild(xmldecl);

            return document;
        }

        public static void SaveDocument(XmlDocument document, FileInfo file) {
            try {
                document.Save(file.FullName);
            }
            catch (FileNotFoundException) {
                throw;
            }
        }

        /**
         * Write Sudoku Inputs to xml file
         */
        private static void SetInputs(uint[,] inputs, XmlDocument document) {
            XmlNode rootNode = document.CreateElement(SudokuXml.RootTag);

            for (int i = 0; i < inputs.GetLength(0); i++) {
                for (int j = 0; j < inputs.GetLength(1); j++) {

                    //position tag
                    XmlNode itemNode = document.CreateElement(SudokuXml.ItemTag);
                    rootNode.AppendChild(itemNode);

                    //row attribute
                    XmlAttribute rowAttr = document.CreateAttribute(SudokuXml.RowAttribute);
                    rowAttr.Value = XmlConvert.ToString(i + 1);
                    itemNode.Attributes.SetNamedItem(rowAttr);

                    //column attribute
                    XmlAttribute colAttr = document.CreateAttribute(SudokuXml.ColumnAttribute);
                    colAttr.Value = XmlConvert.ToString(j + 1);
                    itemNode.Attributes.SetNamedItem(colAttr);

                    //value attribute
                    XmlAttribute valueAttr = document.CreateAttribute(SudokuXml.ValueAttribute);
                    uint value = inputs[i, j];
                    if (value != 0) {
                        valueAttr.Value = XmlConvert.ToString(inputs[i, j]);
                    } else {
                        valueAttr.Value = String.Empty;
                    }
                    itemNode.Attributes.SetNamedItem(valueAttr);
                }
            }
            document.AppendChild(rootNode);
        }
    }
}
