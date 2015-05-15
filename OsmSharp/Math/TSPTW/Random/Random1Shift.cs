﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Math.TSPTW.VNS;
using OsmSharp.Math.VRP.Routes;
using System;

namespace OsmSharp.Math.TSPTW.Random
{
    /// <summary>
    /// An operator to execute n random 1-shift* relocations.
    /// </summary>
    /// <remarks>* 1-shift: Remove a customer and relocate it somewhere.</remarks>
    public class Random1Shift : IPerturber
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "RAN_1SHFT"; }
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="route">The route.</param>
        /// <param name="difference">The difference in fitness.</param>
        /// <returns></returns>
        public bool Apply(IProblem problem, IRoute route, out double difference)
        {
            return this.Apply(problem, route, 1, out difference);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="route">The route.</param>
        /// <param name="level">The level.</param>
        /// <param name="difference">The difference in fitness.</param>
        /// <returns></returns>
        public bool Apply(IProblem problem, IRoute route, int level, out double difference)
        {
            difference = 0;
            var rand = OsmSharp.Math.Random.StaticRandomGenerator.Get();
            var weights = problem.WeightMatrix;
            while(level > 0)
            {
                // remove random customer after another random customer.
                var customer = rand.Generate(problem.Size);
                var insert = rand.Generate(problem.Size - 1);
                if(insert >= customer)
                { // customer is the same of after.
                    insert++;
                }

                // shift after and keep all info.
                int oldBefore, oldAfter, newAfter;
                if(!route.ShiftAfter(customer, insert, out oldBefore, out oldAfter, out newAfter))
                { // shift did not succeed.
                    throw new Exception(
                        string.Format("Failed to shift customer {0} after {1} in route {2}.", customer, insert, route.ToInvariantString()));
                }

                // calculate difference.
                difference = difference -
                    problem.WeightMatrix[oldBefore][customer] -
                    problem.WeightMatrix[customer][oldAfter] +
                    problem.WeightMatrix[oldAfter][oldBefore] -
                    problem.WeightMatrix[insert][newAfter] +
                    problem.WeightMatrix[insert][customer] +
                    problem.WeightMatrix[customer][newAfter];

                // decrease level.
                level--;
            }
            return difference < 0;
        }
    }
}