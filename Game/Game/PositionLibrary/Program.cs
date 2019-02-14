using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PositionLibrary
{
    // Aquesta llibreria serveix per poder instanciar i manipular objectes Position a totes les demes classes de forma generalitzada
    // Per poder fer funcionar la Serialització necessitem la keyword Serialitzable sobre la creació de la classe

    [Serializable]
    public class Position
    {
        public double posX { get; set; }
        public double posY { get; set; }

        public Position(double posX, double posY)
        {
            this.posX = posX;
            this.posY = posY;
        }

        // Els mètodes Serialize i Deserialize ens permeten passar a binari o a objecte (depenent del que volem fer) la informació de Position

        public static byte[] Serialize(object obj)
        {
            byte[] bytesPos;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                bytesPos = ms.ToArray();
            }

            return bytesPos;
        }

        public static object Deserialize(byte[] param)
        {
            object obj = null;
            using (MemoryStream ms = new MemoryStream(param))
            {
                IFormatter br = new BinaryFormatter();
                obj = (br.Deserialize(ms));
            }

            return obj;
        }
    }
}
