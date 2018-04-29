/**
 * This software was developed by Matthew Reilly and Julia Roscher
 * Last Modified: 2017-17-04
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using Aldebaran.Proxies;

namespace NAO_Camera_WPF
{
    class Motion
    {
        MotionProxy naoMotion = null;
        //PostureProxy ALMotion = null;
        /// <summary>
        /// Connects to the motion system in the NAO robot
        /// </summary>
        /// <param name="ip"> ip address of the robot </param>
        public void connect(string ip)
        {
            try
            {
                naoMotion = new MotionProxy(ip, 9559);
                //  naoMotion = new ALProxy();

                // give joints stiffness
                naoMotion.stiffnessInterpolation("Head", 0.6f, 0.6f);
                naoMotion.stiffnessInterpolation("Body", 0.6f, 0.6f);


            }
            catch (Exception e)
            {
                MessageBox.Show("Exception occurred, error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", e.ToString()); // write exepctions to text file
            }
        }

        /// <summary>
        /// Class deconstructor
        /// Cuts motor stiffness
        /// </summary>
        ~Motion()
        {
            if (naoMotion == null)
            {
                return;
            }
            try
            {
                // reduce stiffness
                naoMotion.stiffnessInterpolation("Head", 0.0f, 0.0f);
                naoMotion.stiffnessInterpolation("Body", 0.0f, 0.0f);

            }
            catch (Exception e)
            {
                // display error message and write exceptions to a file
                MessageBox.Show("Exception occurred, error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", e.ToString());
            }
        }

        /// <summary>
        /// Opens the desired hand
        /// </summary>
        /// <param name="hand"> the desired hand, either LHand or RHand </param>
        public void openHand(string hand)
        {
            try
            {
                naoMotion.openHand(hand);
            }
            catch (Exception e)
            {
                // display error message and write exceptions to a file
                MessageBox.Show("Exception occurred, error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", e.ToString());
            }
        }

        /// <summary>
        /// Closes the desired hand
        /// </summary>
        /// <param name="hand"> the desired hand, either LHand or RHand </param>
        public void closeHand(string hand)
        {
            try
            {
                naoMotion.closeHand(hand);
            }
            catch (Exception e)
            {
                // display error message and write exceptions to a file
                MessageBox.Show("Exception occurred, error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", e.ToString());
            }
        }

        /// <summary>
        /// Gets the current angle of a joint
        /// </summary>
        /// <param name="joint"> the joint to retrieve the angle from </param>
        /// <returns> the angle in radians </returns>
        public float getAngle(string joint)
        {
            try
            {
                List<float> angles = naoMotion.getAngles(joint, false);

                return angles[0];
            }
            catch (Exception e)
            {
                // display error message and write exceptions to a file
                MessageBox.Show("Exception occurred, error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", e.ToString());
            }

            return -1;
        }

        /// <summary>
        /// Moves the joint to the desired angle
        /// </summary>
        /// <param name="value"> the angle in radians </param>
        /// <param name="joint"> the joint to be moved </param>
        public void moveJoint(float value, string joint)
        {
            try
            {
                //  naoMotion.setAngles(joint, value, 0.1f);
                naoMotion.angleInterpolation(joint, value, 1, true);
            }
            catch (Exception e)
            {
                // display error message and write exceptions to a file
                MessageBox.Show("Exception occurred, error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", e.ToString());
            }
        }
    }
}