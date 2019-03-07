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
    // Esta librería sirve para poder instanciar y manipular objetos Position en todas las demás clases de forma generalizada
    // Para poder hacer funcionar la Serialización necesitamos la keyword Serialitzable sobre la creación de la clase

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

        // Los métodos Serialize y Deserialize nos permiten pasar a binario o objeto (dependiendo de lo que queremos hacer) la información de Position

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
