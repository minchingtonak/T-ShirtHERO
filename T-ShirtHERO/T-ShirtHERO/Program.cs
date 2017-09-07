using System;
using Microsoft.SPOT;
using System.Threading;

namespace TshirtBot
{
    public class Program
    {
        static CTRE.TalonSrx left = new CTRE.TalonSrx(11);
        static CTRE.TalonSrx leftSlave = new CTRE.TalonSrx(12);
        static CTRE.TalonSrx right = new CTRE.TalonSrx(13);
        static CTRE.TalonSrx rightSlave = new CTRE.TalonSrx(14);

        static CTRE.TalonSrx ledSpike = new CTRE.TalonSrx(15);


        public static void Main()
        {


            CTRE.Gamepad _gamepad = new CTRE.Gamepad(new CTRE.UsbHostDevice());
            //0 - left x
            //1 - left y
            //2 - right x
            //
            //5 - LB
            //6 - RB

            ledSpike.SetControlMode(CTRE.TalonSrx.ControlMode.kVoltage);
            ledSpike.SetCurrentLimit(1);

            while (true)
            {
                float y = -_gamepad.GetAxis(1);
                float turn = _gamepad.GetAxis(2);

                Deadband(ref y);
                Deadband(ref turn);

                if (_gamepad.GetConnectionStatus() == CTRE.UsbDeviceConnection.Connected)
                {
                    if (_gamepad.GetButton(5))
                    {
                        ArcadeDrive(y, turn);
                        /* feed watchdog to keep Talon's enabled */
                        CTRE.Watchdog.Feed();
                    }
                    if (_gamepad.GetButton(6))
                    {
                        ledSpike.Set(3.5f);
                        CTRE.Watchdog.Feed();
                    }
                    else
                    {
                        ledSpike.Set(0);
                    }
                }

                /* run this task every 10ms */
                Thread.Sleep(10);
            }
        }

        static void ArcadeDrive(float moveValue, float rotateValue)
        {
            float leftMotorSpeed;
            float rightMotorSpeed;

            rotateValue = Limit(rotateValue);
            moveValue = Limit(moveValue);

            // Square values for smoothness
            if (rotateValue >= 0.0f)
            {
                rotateValue = rotateValue * rotateValue;
            }
            else
            {
                rotateValue = -(rotateValue * rotateValue);
            }
            if (moveValue >= 0.0f)
            {
                moveValue = moveValue * moveValue;
            }
            else
            {
                moveValue = -(moveValue * moveValue);
            }


            if (rotateValue > 0.0)
            {
                if (moveValue > 0.0)
                {
                    leftMotorSpeed = rotateValue - moveValue;
                    rightMotorSpeed = (float)System.Math.Max(rotateValue, moveValue);
                }
                else
                {
                    leftMotorSpeed = (float)System.Math.Max(rotateValue, -moveValue);
                    rightMotorSpeed = rotateValue + moveValue;
                }
            }
            else
            {
                if (moveValue > 0.0)
                {
                    leftMotorSpeed = -(float)System.Math.Max(-rotateValue, moveValue);
                    rightMotorSpeed = rotateValue + moveValue;
                }
                else
                {
                    leftMotorSpeed = rotateValue - moveValue;
                    rightMotorSpeed = -(float)System.Math.Max(-rotateValue, -moveValue);
                }
            }

            //left.Set(leftMotorSpeed);
            //leftSlave.Set(leftMotorSpeed);
            right.Set(rightMotorSpeed);
            rightSlave.Set(rightMotorSpeed);
        }

        static float Limit(float value)
        {
            if (value > 1.0f)
            {
                return 1.0f;
            }
            if (value < -1.0f)
            {
                return -1.0f;
            }
            return value;
        }

        static void Deadband(ref float value)
        {
            if (value < -0.10)
            {
                /* outside of deadband */
            }
            else if (value > +0.10)
            {
                /* outside of deadband */
            }
            else
            {
                /* within 10% so zero it */
                value = 0;
            }
        }
    }
}
