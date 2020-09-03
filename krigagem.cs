using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace geogenetic
{
    class krigagem
    {
        public static double krigagem2D(double x, double y, double[,] mpt, int tipo, double C0, double C, double a)
        {
            //retorna z estimado
            double[,] meuc1 = meuc(x, y, mpt);
            double r = vestimado(meuc1, tipo, C0, C, a);
            return r;
        }
        static double[,] meuc(double x, double y, double[,] mpt)
        {

            int npontos = Convert.ToInt32(mpt[0, 0]);
            bool[] vbol = new bool[npontos + 1];
            for (int i = 0; i <= npontos; i++) vbol[i] = false;
            double[,] m4pts = new double[5, 4];
            //1o ponto
            double distmin = 1000000000000;
            double xmin = 0, ymin = 0, zmin = 0;
            int indexmin = 0;
            for (int i = 1; i <= npontos; i++)
                if (x != mpt[i, 1] && y != mpt[i, 2] && vbol[i] == false)
                {
                    double x2 = mpt[i, 1];
                    double y2 = mpt[i, 2];
                    double z2 = mpt[i, 3];
                    double dist = dist2d(x, y, x2, y2);
                    if (dist <= distmin)
                    {
                        distmin = dist;
                        xmin = x2;
                        ymin = y2;
                        zmin = z2;
                        indexmin = i;
                    }
                }
            // for (int i = 1;i <= npontos;i++)
            //if (x == xmin && y == ymin) vbol[i] = true;
            vbol[indexmin] = true;
            m4pts[1, 1] = xmin; m4pts[1, 2] = ymin; m4pts[1, 3] = zmin;
            //2o ponto
            distmin = 1000000000000;
            xmin = 0; ymin = 0; indexmin = 0;
            for (int i = 1; i <= npontos; i++)
                if (x != mpt[i, 1] && y != mpt[i, 2] && vbol[i] == false)
                {
                    double x2 = mpt[i, 1];
                    double y2 = mpt[i, 2];
                    double z2 = mpt[i, 3];
                    double dist = dist2d(x, y, x2, y2);
                    if (dist <= distmin)
                    {
                        distmin = dist;
                        xmin = x2;
                        ymin = y2;
                        zmin = z2;
                        indexmin = i;
                    }
                }
            //for (int i = 1;i <= npontos;i++)
            //if (x == xmin && y == ymin) vbol[i] = true;
            vbol[indexmin] = true;
            m4pts[2, 1] = xmin; m4pts[2, 2] = ymin; m4pts[2, 3] = zmin;
            //3o ponto
            distmin = 1000000000000;
            xmin = 0; ymin = 0; indexmin = 0;
            for (int i = 1; i <= npontos; i++)
                if (x != mpt[i, 1] && y != mpt[i, 2] && vbol[i] == false)
                {
                    double x2 = mpt[i, 1];
                    double y2 = mpt[i, 2];
                    double z2 = mpt[i, 3];
                    double dist = dist2d(x, y, x2, y2);
                    if (dist <= distmin)
                    {
                        distmin = dist;
                        xmin = x2;
                        ymin = y2;
                        zmin = z2;
                        indexmin = i;
                    }
                }
            // for (int i = 1;i <= npontos;i++)
            //  if (x == xmin && y == ymin) vbol[i] = true;
            vbol[indexmin] = true;
            m4pts[3, 1] = xmin; m4pts[3, 2] = ymin; m4pts[3, 3] = zmin;
            //4o ponto
            distmin = 1000000000000;
            xmin = 0; ymin = 0; indexmin = 0;
            for (int i = 1; i <= npontos; i++)
                if (x != mpt[i, 1] && y != mpt[i, 2] && vbol[i] == false)
                {
                    double x2 = mpt[i, 1];
                    double y2 = mpt[i, 2];
                    double z2 = mpt[i, 3];
                    double dist = dist2d(x, y, x2, y2);
                    if (dist <= distmin)
                    {
                        distmin = dist;
                        xmin = x2;
                        ymin = y2;
                        zmin = z2;
                        indexmin = i;
                    }
                }
            // for (int i = 1;i <= npontos;i++)
            //   if (x == xmin && y == ymin) vbol[i] = true;
            vbol[indexmin] = true;
            m4pts[4, 1] = xmin; m4pts[4, 2] = ymin; m4pts[4, 3] = zmin;
            double[,] r = new double[5, 7];
            for (int i = 1; i <= 4; i++) for (int j = 1; j <= 4; j++) if (i == j) r[i, j] = 0;

            for (int i = 1; i <= 4; i++) for (int j = 1; j <= 4; j++) if (i != j)
                    {
                        double xi, yi, xj, yj;
                        xi = m4pts[i, 1];
                        yi = m4pts[i, 2];
                        xj = m4pts[j, 1];
                        yj = m4pts[j, 2];
                        r[i, j] = dist2d(xi, yi, xj, yj);
                    }
            for (int i = 1; i <= 4; i++)
            {
                double x2 = m4pts[i, 1];
                double y2 = m4pts[i, 2];
                r[i, 5] = dist2d(x, y, x2, y2);
                r[i, 6] = m4pts[i, 3];
            }
            return r;


        }

        static double vestimado(double[,] meuc, int tipo, double C0, double C, double a)
        {
            //preechendo matriz cov
            double[,] mcov = new double[5, 5];
            for (int i = 1; i <= 4; i++) for (int j = 1; j <= 4; j++)
                {
                    double h = meuc[i, j];
                    double v = funcoes(tipo, C0, C, a, h);
                    mcov[i - 1, j - 1] = v;
                }
            for (int i = 0; i <= 3; i++)
            {
                mcov[i, 4] = 1;
                mcov[4, i] = 1;
            }
            mcov[4, 4] = 0;
            double[] vb = new double[5];
            for (int i = 1; i <= 4; i++)
            {
                double h = meuc[i, 5];
                double v = funcoes(tipo, C0, C, a, h);
                vb[i - 1] = v;
            }
            vb[4] = 1;
            var x = mcov.Solve(vb);
            double soma = 0;
            for (var i = 1; i <= 4; i++) soma = soma + x[i - 1] * meuc[i, 6];

            return soma;


        }

        static double dist2d(double x1, double y1, double x2, double y2)
        {
            double dist = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            return dist;
        }
        static double funcoes(int tipo, double C0, double C, double a, double h)
        {
            double y = 0;
            //esferico
            if (tipo == 1 && h > a) y = C0 + C;
            if (tipo == 1 && h <= a) y = C0 + C * (1.5 * (h / a) - 0.5 * Math.Pow(h / a, 3));
            if (tipo == 2) y = C0 + C * (1 - Math.Exp((-3 * h) / a));
            if (tipo == 3) y = C0 + C * (1 - Math.Exp((-3 * h * h) / (a * a)));
            return y;
        }
    }
}
