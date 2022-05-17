using System.Threading;

namespace EC.Norma.TestUtils
{

    public static class Sequencer
    {
        private static int id;

        public static int GetId() => Interlocked.Increment(ref id);
    }
}
