using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;


namespace RedDot
{
    public class LD220
    {

        private SerialPort sp;
        public LD220(string comport)
        {
            sp = new SerialPort();
            sp.PortName = comport;
            sp.BaudRate = 9600;
            sp.Parity = Parity.None;
            sp.DataBits = 8;
            sp.StopBits = StopBits.One;
        }

        public bool Open()
        {
            try
            {
                sp.Open();
                sp.Write((char)27 + "@");
                sp.Write((char)02 + "C 1");
                return true;
            }
            catch 
            {
           
              // if(e.Message.IndexOf("does not exist") > 0)
               
                   return false;
              

               
            }

        }

        public void Clear()
        {
            Write((char)12 + "");

        }

        public void Close()
        {
            sp.Close();
            sp.Dispose();

        }
        public void Write(string command)
        {
            try
            {
                sp.WriteLine(command);
            }
            catch 
            {
                
            }
        }


        public void DisplayTime()
        {

            Write((char)31 + "U");
        }
    }
}
