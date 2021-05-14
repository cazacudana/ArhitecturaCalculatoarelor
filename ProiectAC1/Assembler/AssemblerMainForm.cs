/***********************************************************************************************
 * Project: Simulator of an microprogrammed Didactical Processor (DP)
 * Module: Assembler for DP
 * Filename: AssemblerMainForm.cs
 * Users: Students in Computer Science (3rd year of study)
 * Creator: Horia V. Caprita
 * Date: 10.03.2015 
***********************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;



namespace Assembler
{
    
    public partial class AssemblerMainForm : Form
    { 
        private String filename;
        List<String> asmGlobalElements = new List<String>();
        List<String> binaryElements = new List<String>();
        String[] BinaryElements=new String[256];
       
       

        Dictionary <string,new_type> Labels = new Dictionary<string, new_type>();
        Dictionary<string, new_type> Brench = new Dictionary<string, new_type>();
        static int PC = 0;
        bool fanion_etichete = false;

        


        int index = 0;
        int contor = 0;
        bool fanion = true;
        bool fanion2 = true;
        Dictionary<int, string> buffer1 = new Dictionary<int, string>();
        Dictionary<int, string> buffer2 = new Dictionary<int, string>();
        Dictionary<int, string> buffer3 = new Dictionary<int, string>();
        //Declaration of instruction format data
        //Instructiuni cu 2 operanzi
        //dimension 2bytes(4b OPCODE 2b MOD ADRESARE  SURSA 4b REGISTRU SURSA  2b MOD ADRESA DESTINATIE 4b REGISTRU DESTINATIE)
        Dictionary<string, string> B1 = new Dictionary<string, string>(){
                   {"MOV" , "0000"},
                   {"ADD" , "0001"},
                   {"SUB" , "0010"},
                   {"CMP" , "0011"},
                   {"AND" , "0100"},
                   {"OR"  , "0101"},
                   {"XOR" , "0110"}
          
        };
        //Instructiuni cu un oprand
        //dimension 2bytes(10b OPCODE 2b MOD ADRESARE  DESTINATIE  4b REGISTRU DESTINATIE  )
        Dictionary<string, string> B2 = new Dictionary<string, string>(){
                   {"CLR" , "1000000000"},
                   {"NEG" , "1000000001"},
                   {"INC" , "1000000010"},
                   {"DEC" , "1000000011"},
                   {"ASL" , "1000000100"},
                   {"ASR" , "1000000101"},
                   {"LSR" , "1000000110"},
                   {"ROL" , "1000000111"},
                   {"ROR" , "1000001000"},
                   {"RLC" , "1000001001"},
                   {"RRC" , "1000001010"},
                   {"JMP",  "1000001011"},
                   {"CALL", "1000001100"},
                   {"PUSH", "1000001101"},
                   {"POP" , "1000001110"}
                              };
        //Instructiuni de salt 
        //2 bytes(8b OPCODE 8b OFFSET
        Dictionary<string, string> B3 = new Dictionary<string, string>(){
                   {"BR"  , "11000000"},
                   {"BNE" , "11000001"},
                   {"BEQ" , "11000010"},
                   {"BPL" , "11000011"},
                   {"BMI" , "11000100"},
                   {"BCS" , "11000101"},
                   {"BCC" , "11000110"},
                   {"BVS" , "11000111"},
                   {"BVC" , "11001000"}
                              };
        //Instructiuni diverse
        //2 bytes (16b OPCODE)
        Dictionary<string, string> B4 = new Dictionary<string, string>(){
                   {"CLC"        , "1110000000000000"},
                   {"CLV"        , "1110000000000001"},
                   {"CLZ"        , "1110000000000010"},
                   {"CLS"        , "1110000000000011"},
                   {"CCC"        , "1110000000000100"},
                   {"SEC"        , "1110000000000101"},
                   {"SEV"        , "1110000000000110"},
                   {"SEZ"        , "1110000000000111"},
                   {"SES"        , "1110000000001000"},
                   {"SCC"        , "1110000000001001"},
                   {"NOP"        , "1110000000001010"},
                   {"RET"        , "1110000000001011"},
                   {"RETI"       , "1110000000001100"},
                   {"HALT"       , "1110000000001101"},
                   {"WAIT"       , "1110000000001110"},
                   {"PUSH_PC"    , "1110000000001111"},
                   {"POP_PC"     , "1110000000010000"},
                   {"PUSH_FLAGS" , "1110000000010001"},
                   {"POP_FLAGS"  , "1110000000010010"}
                              };
        Dictionary<string, string> Registers = new Dictionary<string, string>(){
                   {"R0"  , "0000"},
                   {"R1"  , "0001"},
                   {"R2"  , "0010"},
                   {"R3"  , "0011"},
                   {"R4"  , "0100"},
                   {"R5"  , "0101"},
                   {"R6"  , "0110"},
                   {"R7"  , "0111"},
                   {"R8"  , "1000"},
                   {"R9"  , "1001"},
                   {"R10" , "1010"},
                   {"R11" , "1011"},
                   {"R12" , "1100"},
                   {"R13" , "1101"},
                   {"R14" , "1110"},
                   {"R15" , "1111"}
                              };
        Dictionary<string, string> adressingMode = new Dictionary<string, string>(){
                   {"AM" , "00"},
                   {"AD" , "01"},
                   {"AI" , "10"},
                   {"AX" , "11"}
                              };
        public AssemblerMainForm()
        {
            InitializeComponent();
        }
        string HexConverted(string strBinary)
        {
            string strHex = Convert.ToInt32(strBinary, 2).ToString();
            return strHex;
        }

        public int GetNumeberOfLocationWritten(string []vector)
        {
            int ct = 0;
            for(int i=0;i< vector.Length;i++)
            {
                if(vector[i]!=null)
                {
                    ct++;
                }
            }
            return ct*2;
        }
        public void WriteBinary()
        {
            try
            {
                int size = GetNumeberOfLocationWritten(BinaryElements);
                ushort[] newArray=new ushort [size/2];//creating an array with the same length as the one which holds binary encoding
                string fileName = @"C:\Users\uic77032\sem2\ArhitecturaCalculatoarelor\ProiectAC-branch-3\Assembler\Output.bin";
                int k = 0;
                for(int i=0;i<size;i++)
                {
                    if (BinaryElements[i]!=null)
                    {
                        newArray[k] =Convert.ToUInt16( BinaryElements[i],2);
                        k++;
                    }
                }
                
                using (BinaryWriter binWriter =
                    new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    foreach (var s in newArray)
                    {
                       // Console.WriteLine(s);
                        binWriter.Write(s);
                       



                    }   
                }
               
            }
            catch (IOException ioexp)
            {
                Console.WriteLine("Error: {0}", ioexp.Message);
            }
        }


        /*method to convert a string in to a binary value for 8 bits length*/
        public static string toBinary8(int num)
        {

            int[] p = new int[8];
            string pa = "";
            for (int i = 0; i <= 7; i++)
            {
                p[7 - i] = num % 2;
                num = num / 2;
            }
            for (int i = 0; i <= 7; i++)
            {
                pa += p[i].ToString();
            }
            return pa;
        }
        public static string toSignedBinary8(int num)
        {

            int[] p = new int[8];
            int[] n = new int[8];
            string pa = "";
            string pa2 = "";
            for (int i = 0; i <= 7; i++)
            {
                p[7 - i] = num % 2;
                num = num / 2;
            }

            for (int i = 0; i <= 7; i++)
            {
                pa2 += p[i].ToString();
            }
            //complement fata de 2
            bool cond = false;
            int pozitie = 0 ;
            for (int i = 7; i >= 0; i--)
            {
                if (p[i] == 1 && cond==false)
                {
                    pozitie = i;
                    cond = true;
                }
                
              
            }
            

            for (int k = 7; k >= pozitie; k--)
            {
                n[k] = p[k];
            }
            for (int j = pozitie-1; j >= 0; j--)
            {

                if (p[j] == 1)
                {
                    n[j] = 0;

                }
                else
                {
                    n[j] = 1;
                }
            }

            for (int i = 0; i <= 7; i++)
            {
                pa += n[i].ToString();
            }
            return pa;
        }


        /*method to convert a string in to a binary value for 16 bits length*/
        public static string toBinary16(int num)
        {

            int[] p = new int[16];
            string pa = "";
            for (int i = 0; i <= 15; i++)
            {
                p[15 - i] = num % 2;
                num = num / 2;
            }
            for (int i = 0; i <= 15; i++)
            {
                pa += p[i].ToString();
            }
            return pa;
        }

        //function to convert binary value to hexazecimal
        public static string BinaryStringToHexString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                return binary;

            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

            // TODO: check all 1's or 0's... throw otherwise

            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                //Returns a new string that right-aligns the characters in this instance by padding them on the left with a specified Unicode character,
                //for a specified total length.
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }
        /* Function used to obtain the ASM filename (*.asm) */
        private String getFileName(String filter)
        {
            try
            {
                /* Local variable used to store the filename */
                String fileNameWithPath = "";
                /* Instantiate an OpenFileDialog */
                OpenFileDialog of = new OpenFileDialog();
                /* Set the filter */
                of.Filter = filter;
                /* Get the working directory */
                of.InitialDirectory = Path.GetFullPath("..\\Debug");
                of.RestoreDirectory = true;
                /* Display the Open File dialog */
                if (of.ShowDialog() == DialogResult.OK)
                {
                    /* Get only the filename with full path */
                    fileNameWithPath = of.FileName;
                    /* Get only the filename without path */
                    filename = of.SafeFileName;
                }
                /* Return the filename with complete path */
                return fileNameWithPath;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void ParseFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                /* local variable used for debugging only */
                int lineCounter = 0;
                /* List which will store each token (element) read from ASM file */
                List<String> asmElements = new List<String>();
                /* Create a parser object used for ASM file
                    REMEMBER: this parser can be used for all kind of text files!!!
                 */
                TextFieldParser parser = new TextFieldParser(filename);
                /* Reinitialize the Text property of OutputTextBox */
                OutputTextBox.Text = "";
                /* Define delimiters in ASM file */
                String[] delimiters = {","," "};
                /* Specify that the elements in ASM file are delimited by some characters */
                parser.TextFieldType = FieldType.Delimited;
                /* Set-up the specified delimiters */
                parser.SetDelimiters(delimiters);
                /* Parse the entire ASM file based on previous specifications*/
                while (!parser.EndOfData)
                {//here was pc-2
                    PC += 2;
                    /* Read an entire line in ASM file
                       and split this line in many strings delimited by delimiters */
                    string[] asmFields = parser.ReadFields();
                    /* Store each string as a single element in the list
                       if this string is not empty */
                    foreach (string s in asmFields)
                    {
                        if (!s.Equals(""))
                        {
                            if (s.EndsWith(":"))
                            {//here was pc-2
                                new_type label_state = new new_type(PC-2,false);

                                Labels.Add(s.Split(':')[0],label_state);
                                asmElements.Add(s.Split(':')[0]);
                                asmGlobalElements.Add(s.Split(':')[0]);
                            }
                            else
                            {
                                asmElements.Add(s);
                                //add in to a global list parsed elements of asm file 
                                asmGlobalElements.Add(s);
                               
                            }
                        }
                    }
                    /* Counting the number of lines stored in ASM file */
                    lineCounter++;
                }

                /* Close the parser */
                parser.Close();
                /* If the file is empty, trigger a new exception which
                   in turn will display an error message */
                if (lineCounter == 0)
                {
                    Exception exc = new Exception("The ASM file is empty!");
                    throw exc;
                }
                else
                {
                    /* Display every token in OutputTextBox */
                    foreach (String s in asmElements)
                    {
                        OutputTextBox.Text += s + Environment.NewLine;
                    }
                    /* Display an information about the process completion */
                    MessageBox.Show("Parsing is completed!", "Assembler information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                  
                }

                //verifying if there is any elements in global list ,if the initial asm file have been parsed

                if (asmGlobalElements.Count() != 0)
                {
                    ConvertASMButton.Enabled = true;
                   
                }
                else
                {
                    ConvertASMButton.Enabled = false;
                }

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                /* String used to be displayed in ASMFileTextBox */
                String filename = "";
                /* Reinitialize the Text property of OutputTextBox */
                OutputTextBox.Text = "";
                /* Take the filename selected by user */
                filename = getFileName("ASM file for didactical processor(*.asm)|*.asm");
                /* Display the filename in ASMFileTextBox */
                ASMFileTextBox.Text = filename != null ? filename : ASMFileTextBox.Text;
                /* Enable/Disable the ParseFileButton depending of user choice */
                if (!filename.Equals(""))
                {
                    ParseFileButton.Enabled = true;
                }
                else
                {
                    ParseFileButton.Enabled = false;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AssemblerMainForm_Load(object sender, EventArgs e)
        {

        }
        private void Verify_First_Operand(string opcode, int list_index, string first_operand)
        {   //if  STRING contains () means we have indirect or indexed  addressing
            //ADRESARE INDEXATA

            //trying to take in to account the (r5)14 variant 

            if (first_operand.Contains('(') && first_operand.Contains(')') && first_operand.Split(')')[1] != "")
            {
                try
                {
                    fanion2 = false;
                    //getting the binary codifiation for adressing mode and register
                    String old_value = opcode;
                    String register = first_operand.Split('(', ')')[1];
                    BinaryElements[list_index] = old_value + adressingMode["AX"] + Registers[register];
                   // index++;
                    //getting the binary codifiation for  index
                    BinaryElements[list_index + 2] = toBinary16(Convert.ToUInt16(first_operand.Split(')')[1]));
                    contor += 2;
                    //index++;



                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error in the assembler file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            ////ADRESARE INDIRECTA
            else if (first_operand.Contains('(') && first_operand.Contains(')') && first_operand.Split(')')[1] == "")
            {
                try
                {
                    //getting the binary codifiation for adressing mode and register
                    string register = first_operand.Split('(', ')')[1];
                    BinaryElements[list_index] = opcode + adressingMode["AI"] + Registers[register];
                   // index++;





                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error in the assembler file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            //ADRESARE DIRECTA
            else if (Registers.ContainsKey(first_operand))
            {
                try
                {

                    BinaryElements[list_index] = opcode + adressingMode["AD"] + Registers[first_operand];
                    //index++;

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error in the assembler file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //ADRESARE IMEDIATA-NU POATE FI IMPLEMENTATA PENTRU CA NU PUTEM AVEA VALOAREA IMEDIATA LA REGISTRU DESTINATIE



        }
        private void Verify_Second_Operand(string opcode, int list_index, string second_operand)
        {   //if  STRING contains () means we have indirect or indexed  addressing
            //ADRESARE INDEXATA

            //trying to take in to account the (r5)14 variant 

            if (second_operand.Contains('(') && second_operand.Contains(')') && second_operand.Split(')')[1] != "")
            {
                try
                {
                    fanion = false;
                    
                    //getting the binary codifiation for adressing mode and register
                    String register = second_operand.Split('(', ')')[1];
                    BinaryElements[list_index] = opcode + adressingMode["AX"] + Registers[register];
                   // index++;
                    if (fanion == false && fanion2 == false)
                    {
                        list_index+=2;
                        fanion = true;
                        fanion2 = true;
                    }
                    //getting the binary codifiation for  index
                    BinaryElements[list_index + 2] = toBinary16(Convert.ToUInt16(second_operand.Split(')')[1]));
                    contor += 2;
                    // index++;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error in the assembler file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            ////ADRESARE INDIRECTA
            else if (second_operand.Contains('(') && second_operand.Contains(')') && second_operand.Split(')')[1] == "")
            {
                try
                {
                    //getting the binary codifiation for adressing mode and register
                    String register = second_operand.Split('(', ')')[1];
                    BinaryElements[list_index] = opcode + adressingMode["AX"] + Registers[register];
                  //  index++;

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error in the assembler file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            //ADRESARE DIRECTA
            else if (Registers.ContainsKey(second_operand))
            {

                try
                {
                    BinaryElements[list_index] = opcode + adressingMode["AD"] + Registers[second_operand];
                   // index++;

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error in the assembler file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //ADRESARE IMEDIATA
            else
            {
                try
                {
                    BinaryElements[list_index] = opcode + adressingMode["AM"] + "0000";
                   // index++;
                    BinaryElements[list_index + 2] = toBinary16(Convert.ToUInt16(second_operand));
                    contor += 2;
                    // index++;
                }

                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error in the assembler file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }



        }
        private void Verify_Offset(string opcode, int list_index, string offset)//offeset represent label ,adress of label 
        {   

            //ADRESARE IMEDIATA la adresa data de eticheta
             try
                {
                string signed_value_to_be_written=null;
               
                new_type label_state = new new_type(Labels[offset]);
                index = GetNumeberOfLocationWritten(BinaryElements);
               
                
                if (Convert.ToUInt16((index+2))> Convert.ToUInt16(label_state.GetAdress()))
                {
                    signed_value_to_be_written = toSignedBinary8(Convert.ToUInt16((index+2) - label_state.GetAdress()));
                }
                else if(Convert.ToUInt16((index+2 )) < Convert.ToUInt16(label_state.GetAdress()))
                {
                    signed_value_to_be_written = toBinary8(Convert.ToUInt16(label_state.GetAdress() - (index+2)));
                }
               
                //else
                //{
                //    signed_value_to_be_written = toBinary8(Convert.ToUInt16(index - Labels[offset]));
                //}
                string value_to_be_written= opcode + signed_value_to_be_written;
                BinaryElements[list_index] = value_to_be_written;




                }

                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error in the assembler file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            



        }
        private void Verify_Offset2( int list_index, string offset)//offeset represent label ,adress of label 
        {

            //ADRESARE IMEDIATA la adresa data de eticheta
            try
            {
                string signed_value_to_be_written = null;

                new_type label_state = new new_type(Labels[offset]);//here i will get adress of brench
              

                //if label adress is smaller than brench adress=>-offset
                if (Convert.ToUInt16((list_index)) > Convert.ToUInt16(label_state.GetAdress()))
                {
                    signed_value_to_be_written = toSignedBinary8(Convert.ToUInt16((list_index+2) - label_state.GetAdress()));
                }
                //if label adress is bigger than brench =>+offset
                else if (Convert.ToUInt16((list_index)) < Convert.ToUInt16(label_state.GetAdress()))
                {
                    signed_value_to_be_written = toBinary8(Convert.ToUInt16(label_state.GetAdress()-(list_index+2)));
                }

                
                string value_to_be_written =  signed_value_to_be_written;
                BinaryElements[list_index] = BinaryElements[list_index]+value_to_be_written;




            }

            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error in the assembler file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }




        }
        public void GetOpcode(List<String> asmElements)
        {
            
            bool classB1 = false;
            bool classB2 = false;
            bool classB3 = false;
            bool classB4 = false;
            bool register = false;
            bool number = false;
            bool label = false;
            //contor used to save the number of instruction in list
            //index used like PC
            for (int i = 0; i < asmGlobalElements.Count();)
            {
               
                String s = asmGlobalElements[i];
                
                    if (B1.ContainsKey(s))
                    {
                   
                        classB1 = true;
                    
                        string value = B1[s];
                        int old_contor = contor;
                        Verify_First_Operand(value,contor, asmGlobalElements[i + 1]);
                        string value2 = BinaryElements[old_contor];
                         
                        Verify_Second_Operand(value2, old_contor, asmGlobalElements[i + 2]);
                        i += 3;//used to go to the next instruction element
                        contor+=2;



                     }
                        //instructiuni cu un singur operand 
                    else if (B2.ContainsKey(s))
                    {
                        classB2 = true;
                       // OPCODE.Add(s);
                        String value = B2[s];
                       // binaryElements.Insert(contor, value);
                        Verify_First_Operand(value,contor, asmGlobalElements[i + 1]);
                        // OutputTextBox2.Text += "stringul este de clasa B2 " + classB2 + " " + s +"  Value:"+value +Environment.NewLine;
                         i += 2;
                    contor += 2;
                }
                    //instructiuni de salt 
                    else if (B3.ContainsKey(s))
                    {
                        classB3 = true;
                       
                        String value = B3[s];
                        string offset = asmGlobalElements[i + 1];
                        bool state = Labels[offset].GetState();

                    if (Labels.ContainsKey(offset) && (state==true))
                    {
                        Verify_Offset(value, contor, offset);

                    }
                    else
                    {
                        BinaryElements[contor] = value;
                        new_type brench_state = new new_type(contor,false);
                        Brench.Add(offset, brench_state);//saving those brench that do not have declared an label yet
                    }
                        i+=2;
                    //index += 2;//used to go to the next instruction element
                    contor += 2;//used to now at which instruction  we are 

                }
                    //instructiuni diverse
                    else if (B4.ContainsKey(s))
                    {
                        classB4 = true;
                        // OPCODE.Add(s);
                        String value = B4[s];
                        //binaryElements.Insert(contor,value);
                        BinaryElements[contor] = value;
                        i++;
                    // index += 1;//used to go to the next instruction element
                        contor += 2;//used to now at which instruction  we are 

                }
                   
                    if(classB1==false && classB2==false && classB3==false & classB4==false)
                {
                    if (Labels.ContainsKey(s))
                    {
                        new_type label_state = new new_type(contor, true);

                        Labels[s] = label_state;//reactualize data for labels(adress depending on actual program size and state
                        label_state = Labels[s];//get data(adress ,state)
                        int adress_to_jump = label_state.GetAdress();
                        BinaryElements[contor] = toBinary16(adress_to_jump);
                        contor += 2;
                        label = true;
                        i++;
                    }
                    
                }
                    classB1 = false;
                    classB2 = false;
                    classB3 = false;
                    classB4 = false;
                    label = false;

               


            }

        }

       
       
        public void DisplayBinaryCode(List<String> binaryElements_display)
        {
            if(binaryElements_display.Count > 0)
                {
                foreach (string s in binaryElements_display)
                {
                    OutputTextBox2.Text += "Binary elements  " + " " + s + Environment.NewLine;
                }
            }
        }
        public void DisplayBinaryCodeArray(string[] binaryElements_display)
        {
           
                for(int i=0;i< binaryElements_display.Length;i++)
                {
                    OutputTextBox2.Text += " " + binaryElements_display[i] + Environment.NewLine;
                }
            
        }
        private void ConvertASMButton_Click(object sender, EventArgs e)
        {
            try
            {
                GetOpcode(asmGlobalElements);
                
                foreach (KeyValuePair<string, new_type> kvp in Brench)
                {
                    new_type brench_status=new new_type(kvp.Value);

                    if (brench_status.GetState()==false)
                    {
                        Verify_Offset2(brench_status.GetAdress(),kvp.Key);
                        kvp.Value.SetState(true);
                    }
                    
                }
                DisplayBinaryCodeArray(BinaryElements);
                 WriteBinary();//writing instruction in to binary output file 
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
