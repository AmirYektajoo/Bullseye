﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace BullseyeCursors
{
    public partial class BullseyeForm : Form
    {
        public BullseyeForm()
        {
            target = new Target();
            xCursor = new Cursor(400, 10);
            yCursor = new Cursor(10, 400);
            InitializeComponent();
        }

        private readonly Target target;
        private readonly Cursor xCursor;
        private readonly Cursor yCursor;
        
        private int points;
        private int attempts = 5;
            
        private const double CenterX = 200;
        private const double CenterY = 200;
        
        private bool greenFlag;
        private bool yellowFlag;
        private bool redFlag;
        private bool blueFlag;
        private bool blackFlag;

        private int xCoordinate;
        private int yCoordinate;
        private int spaceKeyPressedCounter;

        /// <summary>
        /// This method sets up the UI and visual elements of the application.
        /// Whenever the form is loaded, all the visual elements will be displayed.
        /// Also, the cursor on x is moving when the application is launched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeLabels();
            DrawNewImages();
            StartXTimer();
        }

        /// <summary>
        /// Deals with the design of the labels.
        /// </summary>
        private void InitializeLabels()
        {
            InitializeAttemptsLabel();
            InitializePointsLabel();
            InitializeInstructionsLabel();
        }

        private void DrawNewImages()
        {
            targetPictureBox.Image = target.DrawNew();
            DrawNewCursors();
        }

        private void DrawNewCursors()
        {
            xCursorPictureBox.Image = xCursor.DrawNew();
            yCursorPictureBox.Image = yCursor.DrawNew();
        }

        private void StartXTimer()
        {
            xCursorTimer.Interval = 1;
            xCursorTimer.Enabled = true;
            xCursorTimer.Start();
        }

        private void StartYTimer()
        {
            yCursorTimer.Interval = 1;
            yCursorTimer.Enabled = true;
            yCursorTimer.Start();
        }

        private void XCursorTimer_Tick(object sender, EventArgs e)
        {
            xCursorPictureBox.Image = xCursor.DrawOnTickAndUpdateCoordinateValue();
        }

        private void YCursorTimer_Tick(object sender, EventArgs e)
        {
            yCursorPictureBox.Image = yCursor.DrawOnTickAndUpdateCoordinateValue();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // ESCAPE
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.Close();
            }
            
            // RETRY
            if ((Keys)e.KeyValue == Keys.R)
            {
                DrawNewImages();

                points = 0;
                pointsLabel.Text = StringResources.GetPointsText(points);

                attempts = 5;
                attemptsLabel.Text = StringResources.GetAttemptsText(attempts);

                spaceKeyPressedCounter = 0;
                StartXTimer();
            }

            // SPACE
            if((Keys)e.KeyValue == Keys.Space)
            {
                ++spaceKeyPressedCounter;

                if(attempts > 0 && spaceKeyPressedCounter == 1)
                {
                    xCursorTimer.Stop();
                    xCursorTimer.Enabled = false;
                    if (xCursor.Coordinate - xCursor.PreviousCoordinate >= 0)
                        xCoordinate = xCursor.Coordinate - 7;
                    else if (xCursor.Coordinate - xCursor.PreviousCoordinate < 0)
                        xCoordinate = xCursor.Coordinate + 14;
                    xCursorPictureBox.Image = xCursor.Bitmap;

                    StartYTimer();
                }

                if (attempts > 0 && spaceKeyPressedCounter == 2)
                {
                    yCursorTimer.Stop();
                    yCursorTimer.Enabled = false;
                    if (yCursor.Coordinate - yCursor.PreviousCoordinate >= 0)
                        yCoordinate = yCursor.Coordinate - 7;
                    else if (yCursor.Coordinate - yCursor.PreviousCoordinate < 0)
                        yCoordinate = yCursor.Coordinate + 14;
                    yCursorPictureBox.Image = yCursor.Bitmap;
                    
                    timer.Enabled = true;
                    timer.Start();
                    timer.Interval = (750);

                    var pointsToAdd = 0;
                    // Green area 100p
                    if (Math.Sqrt(Math.Pow(xCoordinate - CenterX, 2f) + Math.Pow(yCoordinate - CenterY, 2f)) < 10)
                    {
                        pointsToAdd = 100;
                        greenFlag = true;
                        DrawHoleInTarget();
                    }

                    // Yellow area 50p
                    if ((Math.Sqrt(Math.Pow(xCoordinate - CenterX, 2f) + Math.Pow(yCoordinate - CenterY, 2f)) > 10) &&
                        (Math.Sqrt(Math.Pow(xCoordinate - CenterX, 2f) + Math.Pow(yCoordinate - CenterY, 2f)) < 50))
                    {
                        pointsToAdd = 50;
                        yellowFlag = true;
                        DrawHoleInTarget();
                    }

                    // Red area 25p
                    if ((Math.Sqrt(Math.Pow(xCoordinate - CenterX, 2f) + Math.Pow(yCoordinate - CenterY, 2f)) > 50) &&
                        (Math.Sqrt(Math.Pow(xCoordinate - CenterX, 2f) + Math.Pow(yCoordinate - CenterY, 2f)) < 100))
                    {
                        pointsToAdd = 25;
                        redFlag = true;
                        DrawHoleInTarget();
                    }

                    // Blue area 10p
                    if ((Math.Sqrt(Math.Pow(xCoordinate - CenterX, 2f) + Math.Pow(yCoordinate - CenterY, 2f)) > 100) &&
                        (Math.Sqrt(Math.Pow(xCoordinate - CenterX, 2f) + Math.Pow(yCoordinate - CenterY, 2f)) < 150))
                    {
                        pointsToAdd = 10;
                        blueFlag = true;
                        DrawHoleInTarget();
                    }

                    // Black area 0p
                    if ((Math.Sqrt(Math.Pow(xCoordinate - CenterX, 2f) + Math.Pow(yCoordinate - CenterY, 2f)) > 150) &&
                        (Math.Sqrt(Math.Pow(xCoordinate - CenterX, 2f) + Math.Pow(yCoordinate - CenterY, 2f)) < 200))
                    {
                        blackFlag = true;
                        DrawHoleInTarget();
                    }
                    
                    pointsLabel.Text = StringResources.GetPointsWithPointsToAddText(points, pointsToAdd);
                    attemptsLabel.Text = StringResources.GetAttemptsWithMinusOneText(attempts);
                    spaceKeyPressedCounter = 0;
                }
            }
        }

        /// <summary>
        /// Draws a hole at the of (x, y) coordinates.
        /// </summary>
        private void DrawHoleInTarget()
        {
            targetPictureBox.Image = target.DrawHoleAt(xCoordinate, yCoordinate);
        }

        /// <summary>
        /// Makes label display dynamic.
        /// Resets cursors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (greenFlag)
            {
                points += 100;
                greenFlag = false;
            }

            if (yellowFlag)
            {
                points += 50;
                yellowFlag = false;
            }

            if (redFlag)
            {
                points += 25;
                redFlag = false;
            }

            if (blueFlag)
            {
                points += 10;
                blueFlag = false;
            }

            if (blackFlag)
            {
                points += 0;
                blackFlag = false;
            }
            
            --attempts;
            pointsLabel.Text = StringResources.GetPointsText(points);
            attemptsLabel.Text = StringResources.GetAttemptsText(attempts);
            
            timer.Stop();
            timer.Enabled = false;


            if(attempts > 0)
            {
                spaceKeyPressedCounter = 0;
                DrawNewCursors();
                StartXTimer();
            }
        }
        
        private void InitializeAttemptsLabel()
        {
            attemptsLabel.Location = new Point(50, 0);
            attemptsLabel.Size = new Size(100, 50);
            attemptsLabel.AutoSize = true;
            attemptsLabel.Text = StringResources.GetAttemptsText(attempts);
        }

        private void InitializePointsLabel()
        {
            pointsLabel.Location = new Point(250, 0);
            pointsLabel.Size = new Size(100, 50);
            pointsLabel.AutoSize = true;
            pointsLabel.Text = StringResources.GetPointsText(points);
        }

        private void InitializeInstructionsLabel()
        {
            InstructionsLabel.AutoSize = true;
            InstructionsLabel.Text = StringResources.GetInstructions();
        }
    }
}
