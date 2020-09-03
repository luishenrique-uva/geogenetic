using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace geogenetic
{
    public static class sa
    {
        static double T0 = 500;
        static int np = 500;
        static Random aleat = new Random((int)DateTime.Now.Ticks);

        public static double[] simulated(double[] vx, double[] vy)
        {
            double[] r = regressao(vx, vy);
            return r;
        }

        public static double funcoes(int tipo, double C0, double C, double a, double h)
        {
            double y = 0;
            //esferico
            if (tipo == 1 && h > a) y = C0 + C;
            if (tipo == 1 && h <= a) y = C0 + C * (1.5 * (h / a) - 0.5 * Math.Pow(h / a, 3));
            if (tipo == 2) y = C0 + C * (1 - Math.Exp((-3 * h) / a));
            if (tipo == 3) y = C0 + C * (1 - Math.Exp((-3 * h * h) / (a * a)));
            return y;
        }
        static double loss(int tipo, double[] vx, double[] vy, double C0, double C, double a)
        {
            double soma, aux, ycalc, yobs;
            int n = vx.Length - 1;

            soma = 0;
            for (var i = 1; i <= n; i++)
            {
                ycalc = funcoes(tipo, C0, C, a, vx[i]);
                yobs = vy[i];
                aux = Math.Pow(yobs - ycalc, 2);
                soma = soma + aux;
            }

            return soma;
        }
        static double[] simulated_local(int tipo, double[] vx, double[] vy, int np, double T0)
        {

            double C01, C02, C03, C1, C2, C3, a1, a2, a3, f1, f2, f3, aux1, aux2;
            double C0f = 0, Cf = 0, af = 0, ff = 0;
            int nh = vx.Length - 1;



            //inicializando parametros
            //inicio:
            aleat = new Random((int)DateTime.Now.Ticks);
            // C03 = 0;C3 = 0;a3 = 0;
            double T = T0;
        inicioloop:
            aux1 = vy[1] - (vx[1] / (vx[2] - vx[1])) * (vy[2] - vy[1]);
            C01 = Math.Max(0, aux1);

            a1 = vx[nh] / 2;
            C1 = (vy[nh - 2] + vy[nh - 1] + vy[nh]) / 3 - C01;
            //if (aux1 < 0) aux1 = vy[nh] - C01;
            //C1 = Math.Max(1, aux1);



            for (var i = 1; i <= 50000; i++)
            {
                //gerando vizinho (pertubação) até 10% para mais ou para menos
                C02 = (0.9 + aleat.NextDouble() / 5) * C01;
                if (C02 < 0) C02 = 0;
                if (C02 > vy.Max()) C02 = vy.Max();
                C2 = (0.9 + aleat.NextDouble() / 5) * C1;
                if (C2 > vy.Max()) C02 = vy.Max();
                if (C2 < vy[2]) C02 = vy[2];
                a2 = (0.9 + aleat.NextDouble() / 5) * a1;
                if (a2 < vx[2]) a2 = vx[2];
                f1 = loss(tipo, vx, vy, C01, C1, a1);
                f2 = loss(tipo, vx, vy, C02, C2, a2);
                if (f2 < f1)
                {
                    C01 = C02; C1 = C2; a1 = a2;
                    // C03 = C02; C3 = C2; a3 = a2;
                }
                //if (f2 >= f1 && aleat.NextDouble() < Math.Exp((f1 - f2) / T))
                //{
                //    C03 = C01; C3 = C1; a3 = a1;
                //    C01 = C02; C1 = C2; a1 = a2;
                //}
            }




            //caso os parâmetros fiquem iguais a zero , repetir procedimento...


            f1 = loss(tipo, vx, vy, C01, C1, a1);

            if (f1 == 0 || C1 == 0 || a1 == 0)
            {
                goto inicioloop;
            }

            //if (f1 < 0.01)
            //{
            //    Console.WriteLine(" erro regressao...");
            //    goto inicio;
            //}
            double[] r = new double[4];
            r[0] = f1;
            r[1] = C01;
            r[2] = C1;
            r[3] = a1;
            return r;

        }
        static double[] simulated_local2(int tipo, double[] vx, double[] vy, int np, double T0)
        {

            double C01, C02, C03, C1, C2, C3, a1, a2, a3, f1, f2, f3, aux1, aux2;
            double C0f = 0, Cf = 0, af = 0, ff = 0;
            int nh = vx.Length - 1;



            //inicializando parametros
            //inicio:
            aleat = new Random((int)DateTime.Now.Ticks);
            // C03 = 0;C3 = 0;a3 = 0;
            double T = T0;
            aux1 = vy[1] - (vx[1] / (vx[2] - vx[1])) * (vy[2] - vy[1]);
            C01 = Math.Max(0, aux1);

            a1 = vx[nh] / 2;
            C1 = (vy[nh - 2] + vy[nh - 1] + vy[nh]) / 3 - C01;
            //if (aux1 < 0) aux1 = vy[nh] - C01;
            //C1 = Math.Max(1, aux1);

            while (T > 0.1)
            {
                for (var i = 1; i <= np; i++)
                {
                    //gerando vizinho (pertubação) até 10% para mais ou para menos
                    C02 = (0.9 + aleat.NextDouble() / 5) * C01;
                    if (C02 < 0) C02 = 0;
                    if (C02 > vy.Max()) C02 = vy.Max();
                    C2 = (0.9 + aleat.NextDouble() / 5) * C1;
                    if (C2 > vy.Max()) C02 = vy.Max();
                    a2 = (0.9 + aleat.NextDouble() / 5) * a1;
                    if (a2 < vx[2]) a2 = vx[2];
                    f1 = loss(tipo, vx, vy, C01, C1, a1);

                    f2 = loss(tipo, vx, vy, C02, C2, a2);
                    if (f2 < f1)
                    {
                        C01 = C02; C1 = C2; a1 = a2;
                        // C03 = C02; C3 = C2; a3 = a2;
                    }
                    //if (f2 >= f1 && aleat.NextDouble() < Math.Exp((f1 - f2) / T))
                    //{
                    //    C03 = C01; C3 = C1; a3 = a1;
                    //    C01 = C02; C1 = C2; a1 = a2;
                    //}
                }
                T = 0.95 * T;
            }

            //caso os parâmetros fiquem iguais a zero , repetir procedimento...


            f1 = loss(tipo, vx, vy, C01, C1, a1);
            //if (f1 < 0.01)
            //{
            //    Console.WriteLine(" erro regressao...");
            //    goto inicio;
            //}
            double[] r = new double[4];
            r[0] = f1;
            r[1] = C01;
            r[2] = C1;
            r[3] = a1;
            return r;

        }
        static double[] regressao(double[] vx, double[] vy)
        {

            double[] vsim1 = simulated_local(1, vx, vy, np, T0);
            double[] vsim2 = simulated_local(2, vx, vy, np, T0);
            double[] vsim3 = simulated_local(3, vx, vy, np, T0);
            double[] r = new double[5];
            //1 melhor
            if (vsim1[0] < vsim2[0] && vsim1[0] < vsim3[0])
            {
                r[0] = 1;
                r[1] = vsim1[0];
                r[2] = vsim1[1];
                r[3] = vsim1[2];
                r[4] = vsim1[3];
            }
            //2 melhor
            if (vsim2[0] < vsim1[0] && vsim2[0] < vsim3[0])
            {
                r[0] = 2;
                r[1] = vsim2[0];
                r[2] = vsim2[1];
                r[3] = vsim2[2];
                r[4] = vsim2[3];
            }
            //3 melhor
            if (vsim3[0] < vsim1[0] && vsim3[0] < vsim2[0])
            {
                r[0] = 3;
                r[1] = vsim3[0];
                r[2] = vsim3[1];
                r[3] = vsim3[2];
                r[4] = vsim3[3];
            }


            return r;
        }
    }
}
