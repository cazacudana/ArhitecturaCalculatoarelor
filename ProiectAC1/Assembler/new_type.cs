using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    class new_type
    {
        int address;
        bool state;
        public new_type(int x, bool y)
        {
            address = x;
            state = y;

        }
        public new_type(new_type param)
        {
            this.address = param.address;
            this.state = param.state;

        }
        public int GetAdress()
        {
            return this.address;
        }
        public bool GetState()
        {
            return this.state;
        }
        public void SetAddress(int address)
        {
            this.address = address;
        }
        public void SetState(bool state)
        {
            this.state = state;
        }
    }
}
