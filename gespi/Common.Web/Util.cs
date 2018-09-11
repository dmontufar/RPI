using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Common.Web
{
    public static class Util
    {
        public static IEnumerable<SelectListItem> GetListOfItems(string collection)
        {
            var listOfItems = new List<System.Web.Mvc.SelectListItem>();
            var tmp = collection.Split(',').ToList();

            for (int i = 0; i < tmp.Count; i++)
            {
                listOfItems.Add(new SelectListItem() { Value = tmp[i], Text = tmp[i + 1] });
                i++;
            }
            return listOfItems;
        }

        public static string GetAppSetting(string apiKey)
        {
            var value = string.Empty;

            value = ConfigurationManager.AppSettings[apiKey];

            return value;
        }

        private static Random random = new Random((int)DateTime.Now.Ticks);

        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;

            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static string getMes(int nMes)
        {
            var aMeses = new string[12] { "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };
            return aMeses[nMes - 1];
        }

        public static string getEnLetras(int nNumero)
        {
            var aletras = new string[16] { "CERO", "UNO", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE", "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE" };
            var aletras2 = new string[9] { "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };

            var enLetras = "";
            var nY_N = 0;

            if (nNumero < 16)
            {
                enLetras = aletras[nNumero];
            }
            else
            {
                nY_N = (nNumero % 10);
                if (nY_N == 0)
                    enLetras = aletras2[((nNumero - nY_N) / 10) - 1];
                else
                    enLetras = aletras2[((nNumero - nY_N) / 10) - 1] + " Y " + aletras[nY_N];
            }

            return enLetras;
        }

        // numeros grandes
        public static string getNumeroEnLetras(double _xValor_)
        {
            var ret_str = "";

            var entero = _xValor_;
            if (entero > 0)
            {
                var millones = (int)Math.Truncate(entero / 1000000);

                if (millones > 0)
                {
                    var miles = millones / 1000;
                    var cientos = millones % 1000;

                    if (miles > 0)
                        ret_str = DEUNOANOVE(miles) + " Mil ";

                    if (cientos > 0)
                        ret_str = ret_str + DEUNOANOVE(cientos);

                    if (millones > 1)
                        ret_str = ret_str + " Millones ";
                    else
                        ret_str = ret_str + " Millon ";

                }

                var miles_ = entero % 1000000;

                if (miles_ > 0)
                {
                    var miles = Convert.ToInt32(miles_ / 1000);
                    var cientos = Convert.ToInt32(Math.Truncate(miles_ % 1000));
                    if (miles > 0)
                        ret_str = ret_str + DEUNOANOVE(miles) + " Mil ";

                    if (cientos > 0)
                        ret_str = ret_str + DEUNOANOVE(cientos);

                }
            }
            else
            {
                ret_str = "CERO ";
            }

            return ret_str;
        }


        private static string DEUNOANOVE(int _xValor_) // _001_900
        {
            int centena = Convert.ToInt32(_xValor_ / 100);
            var decenas = _xValor_ % 100;
            var ret_str = "";
            if (centena > 0)
            {
                ret_str = DECIENANOVE(centena);
                if (centena == 1 && decenas > 0)
                    ret_str = ret_str + "to";

                ret_str = ret_str + " ";
            }

            if (decenas > 0)
            {
                if (decenas >= 10 & decenas <= 15)
                {
                    ret_str = ret_str + DEDIEZAQUI(decenas);
                }
                else
                {
                    var unidades = decenas % 10;
                    decenas = Convert.ToInt32(decenas / 10);
                    ret_str = ret_str + DEUNOACIEN(decenas, unidades);
                }
            }
            return ret_str;
        }

        private static string DECIENANOVE(int _xValor_) //_100_900
        {
            if (_xValor_ == 1)
                return "Cien";
            else if (_xValor_ == 2)
                return "Doscientos";
            else if (_xValor_ == 3)
                return "Trescientos";
            else if (_xValor_ == 4)
                return "Cuatrocientos";
            else if (_xValor_ == 5)
                return "Quinientos";
            else if (_xValor_ == 6)
                return "Seiscientos";
            else if (_xValor_ == 7)
                return "Setecientos";
            else if (_xValor_ == 8)
                return "Ochocientos";
            else if (_xValor_ == 9)
                return "Novecientos";
            return "";
        }

        private static string DEDIEZAQUI(int _xValor_) // _010_015
        {
            if (_xValor_ == 10)
                return "Diez";
            else if (_xValor_ == 11)
                return "Once";
            else if (_xValor_ == 12)
                return "Doce";
            else if (_xValor_ == 13)
                return "Trece";
            else if (_xValor_ == 14)
                return "Catorce";
            else if (_xValor_ == 15)
                return "Quince";
            return "";
        }


        private static string DEUNOACIEN(int decenas, int unidades) //_001_100
        {
            var ret_str = "";
            if (decenas == 1 && unidades == 0)
                ret_str = "Diez";
            else if (decenas == 1)
                ret_str = "Dieci";
            else if (decenas == 2 && unidades == 0)
                ret_str = "Veinte";
            else if (decenas == 2)
                ret_str = "Veinti";
            else if (decenas == 3)
                ret_str = "Treinta";
            else if (decenas == 4)
                ret_str = "Cuarenta";
            else if (decenas == 5)
                ret_str = "Cincuenta";
            else if (decenas == 6)
                ret_str = "Sesenta";
            else if (decenas == 7)
                ret_str = "Setenta";
            else if (decenas == 8)
                ret_str = "Ochenta";
            else if (decenas == 9)
                ret_str = "Noventa";

            if (decenas > 2 && unidades > 0)
                ret_str = ret_str + " y ";

            if (unidades > 0)
                ret_str = ret_str + DEUNOANUEVE(unidades);

            return ret_str;
        }



        private static string DEUNOANUEVE(int _xValor_) //&&_001_009
        {
            if (_xValor_ == 1)
                return "Uno";
            else if (_xValor_ == 2)
                return "Dos";
            else if (_xValor_ == 3)
                return "Tres";
            else if (_xValor_ == 4)
                return "Cuatro";
            else if (_xValor_ == 5)
                return "Cinco";
            else if (_xValor_ == 6)
                return "Seis";
            else if (_xValor_ == 7)
                return "Siete";
            else if (_xValor_ == 8)
                return "ocho";
            else if (_xValor_ == 9)
                return "Nueve";
            return "";

        }
    }
}