using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfGenerator
{
    public static class Util
    {
        public static string FormatMoneyPtBr(this decimal value)
        {
            var valueConvert = value.ToString();
            if(string.IsNullOrEmpty(valueConvert))
            {
                return null;
            }

            var split = valueConvert.Split(",");

            if(split.Length == 1)
            {
                split = new string[] { valueConvert, "00" };
            }

            var principal = split[0];

            if(principal.Length > 3 && !principal.Contains("."))
            {
                var list = new List<char>();
                var array = principal.Reverse().ToArray();
                var cont = 0;
                foreach(var c in array)
                {
                    if(cont == 3)
                    {
                        list.Add('.');
                        cont = 0;
                    }
                    list.Add(c);
                    cont++;
                }

                list.Reverse();
                principal = new string(value: list.ToArray());
            }

            var cents = split[1];

            if(cents.Length == 1)
            {
                cents += "0";
            }

            return $"R$ {principal},{cents}";
        }

        public static string DecimalToExtenso(this decimal valor)
        {
            if(valor == 0)
            {
                return "ZERO REAIS";
            }
            string[] struni = new string[] { "", "Um", "Dois", "Tres", "Quatro", "Cinco", "Seis", "Sete", "Oito", "Nove", "Dez", "Onze", "Doze", "Treze", "Quatorze", "Quinze", "Dezessis", "Dezessete", "Dezoito", "Dezenove", "Vinte" };
            string[] strdez = new string[] { "", "", "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" };
            List<string[]> strcen = new List<string[]>() { new string[] { "", "" }, new string[] { "Cem", "Cento" }, new string[] { "Duzentos", "Duzentos" }, new string[] { "Trezentos", "Trezentos" }, new string[] { "Quatrocentos", "Quatrocentos" }, new string[] { "Quinhentos", "Quinhentos" }, new string[] { "Seiscentos", "Seiscentos" }, new string[] { "Setecentos", "Setecentos" }, new string[] { "Oitocentos", "Oitocentos" }, new string[] { "Novecentos", "Novecentos" } };
            List<string[]> moeda = new List<string[]>() { new string[] { " Trilhao", " Trilhoes" }, new string[] { " Bilhao", " Bilhoes" }, new string[] { " Milhao", " Milhoes" }, new string[] { " Mil", " Mil" }, new string[] { " Real", " Reais" }, new string[] { " Centavo", " Centavos" }, };
            List<string[]> result = new List<string[]>();
            string[] arrValor = Decimal.Round(valor, 2).ToString("0|0|0,0|0|0,0|0|0,0|0|0,0|0|0.0|0").Replace(",", ",0|").Replace(",", ".").Split('.');
            for(int i = arrValor.Length - 1; i >= 0; i--)
            {
                string[] z = arrValor[i].Split('|');
                int a = Convert.ToInt32(z[0]);
                int b = Convert.ToInt32(z[1]);
                int c = Convert.ToInt32(z[2]);
                int d = Convert.ToInt32(b.ToString() + c.ToString());
                int k = Convert.ToInt32(a.ToString() + b.ToString() + c.ToString());
                string r = (d >= 1 && d <= 20) ? string.Format("{0}", k == 0 ? "" : struni[d]) : string.Format("{0}{1}{2}", strdez[b], c > 0 ? " e " : "", k == 0 ? "" : struni[c]);
                r = k < 100 ? r : string.Format("{0}{1}{2}", strcen[a][d == 0 ? 0 : 1], d == 0 ? "" : " e ", r);
                result.Add(new string[] { i.ToString(), k.ToString(), r, " e ", moeda[i][k == 1 ? 0 : 1] });
            }
            if(Convert.ToInt32(result[1][1]) == 0)
            {
                string xmoeda = result[1][4];
                for(int i = 2; i <= result.Count - 1; i++)
                {
                    if(Convert.ToInt32(result[i][1]) > 0)
                    {
                        result[i][4] += " " + (i == 3 || i == 4 || i == 5 ? " de " : "") + xmoeda;
                        result[i][3] = " e ";
                        break;
                    }
                }
            }
            for(int i = result.Count - 1; i >= 0; i--) { if(Convert.ToInt32(result[i][1]) == 0) result.Remove(result[i]); }
            result[0][3] = "";
            for(int i = 2; i <= result.Count - 1; i++) { result[i][3] = ", "; }
            string extenso = "";
            for(int i = 0; i <= result.Count - 1; i++) { extenso = result[i][2] + result[i][4] + result[i][3] + extenso; }
            return extenso.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").ToUpper(); ;
        }
    }
}