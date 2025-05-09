﻿namespace HMD.Scripts.Util
{
    using System;
    public class Frac
    {
        private int _nominator;
        private int _denominator;

        public Frac(int nominator, int denominator)
        {
            _nominator = nominator;
            _denominator = denominator;
        }

        public double ToDouble()
        {
            return (double)_nominator / _denominator;
        }
        public double ToLn()
        {
            return Math.Log(ToDouble());
        }

        public static Frac FromDouble(double d)
        {
            // Multiply the aspect ratio by 100 to produce a whole number
            var wholeNumber = (int)(d * gcdBase);

            // Find the GCD of the whole number and 100
            var gcd = GCD(wholeNumber, gcdBase);

            // Divide the whole number and 100 by the GCD to reduce the fraction to its lowest terms
            var numerator = wholeNumber / gcd;
            var denominator = gcdBase / gcd;

            return new Frac(numerator, denominator);
        }
        
        public static Frac FromLn(double d)
        {
            return FromDouble(Math.Exp(d));
        }

        private static int gcdBase = 128;

        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                var t = b;
                b = a % b;
                a = t;
            }

            return a;
        }

        public override string ToString()
        {
            return $"{_nominator}/{_denominator}";
        }
        public string ToRatioText()
        {
            return $"{_nominator}:{_denominator}";
        }

        public static Frac FromRatioText(string text)
        {
            var split = text.Split(':');
            var a = int.Parse(split[0]);
            var b = int.Parse(split[1]);
            return new Frac(a, b);
        }

        // generated by Copilot
        public static Frac operator +(Frac a, Frac b)
        {
            return new Frac(a._nominator * b._denominator + b._nominator * a._denominator,
                a._denominator * b._denominator);
        }

        public static Frac operator -(Frac a, Frac b)
        {
            return new Frac(a._nominator * b._denominator - b._nominator * a._denominator,
                a._denominator * b._denominator);
        }

        public static Frac operator *(Frac a, Frac b)
        {
            return new Frac(a._nominator * b._nominator, a._denominator * b._denominator);
        }

        public static Frac operator /(Frac a, Frac b)
        {
            return new Frac(a._nominator * b._denominator, a._denominator * b._nominator);
        }

        public static Frac operator -(Frac a)
        {
            return new Frac(-a._nominator, a._denominator);
        }


    }
}
