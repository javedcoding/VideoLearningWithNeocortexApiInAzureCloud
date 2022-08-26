namespace MyExperiment.Utilities
{
    public static class ArrayUtils
    {

        /**
         * Returns an array whose members are the quotient of the dividend array
         * values and the divisor value.
         *
         * @param dividend
         * @param divisor
         * @param dividend adjustment
         * @param divisor  adjustment
         *
         * @return
         * @throws IllegalArgumentException if the two argument arrays are not the same length
         */
        public static double[] divide(double[] dividend, double divisor)
        {
            double[] quotient = new double[dividend.Length];
            double denom = 1;
            for (int i = 0; i < dividend.Length; i++)
            {
                quotient[i] = (dividend[i]) /
                              (double)((denom = divisor) == 0 ? 1 : denom); //Protect against division by 0
            }
            return quotient;
        }

        /**
         * Returns the passed in array with every value being altered
         * by the addition of the specified double amount at the same
         * index
         *
         * @param arr
         * @param amount
         * @return
         */
        public static double[] AddOffset(double[] arr, double[] offset)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] += offset[i];
            }
            return arr;
        }
    }
}