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

using System;
using System.Collections.Generic;
using System.Text;

namespace OsmSharp.Math.VRP.Routes
{
    /// <summary>
    /// Represents a route encapsulating an IRoute but with depot included.
    /// </summary>
    public class DepotRoute : IRoute
    {
        /// <summary>
        /// Holds the encapsulated route.
        /// </summary>
        private IRoute _route;

        /// <summary>
        /// Creates a new depot route.
        /// </summary>
        /// <param name="route"></param>
        public DepotRoute(IRoute route)
        {
            _route = route;
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsEmpty
        {
            get { return false; }
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        public bool IsRound
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the number of customers.
        /// </summary>
        public int Count
        {
            get { return _route.Count + 1; }
        }

        /// <summary>
        /// Returns the first customer.
        /// </summary>
        public int First
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        public int Last
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns true if the edge is contained in this route.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool Contains(int from, int to)
        {
            if (from == 0 || to == 0)
            {
                return (_route.First == to && from == 0) ||
                    (_route.Last == from && to == 0);
            }
            return _route.Contains(from, to);
        }

        /// <summary>
        /// Returns true if the customer is in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Contains(int customer)
        {
            if (customer == 0)
            {
                return true;
            }
            return _route.Contains(customer);
        }

        /// <summary>
        /// Removes a given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Remove(int customer)
        {
            if (customer == 0)
            {
                throw new Exception("Cannot remove depot from depot routes.");
            }
            return _route.Remove(customer);
        }

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to.
        /// 0->1->2:ReplaceEdgeFrom(0, 2):0->2 without resetting the last customer property.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void ReplaceEdgeFrom(int from, int to)
        {
            if (to == 0 && from == 0)
            { // clear all elements from the route.
                _route.Clear();
            }
            else if (to == 0)
            {
                _route.ReplaceEdgeFrom(from, -1);
            }
            else if (from == 0)
            { // TODO: replace the from customer.
                _route.ReplaceFirst(to);
            }
            else
            {
                _route.ReplaceEdgeFrom(from, to);
            }
        }

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to->unknown.
        /// 0->1:InsertAfter(0, 2):0->2-1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void InsertAfter(int from, int to)
        {
            if (from == 0)
            {
                _route.InsertFirst(to);
            }
            else
            {
                _route.InsertAfter(from, to);
            }
        }

        /// <summary>
        /// Returns the neighbours of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int[] GetNeigbours(int customer)
        {
            //int[] neighbour;
            //if (customer == 0)
            //{
            //    neighbour = new int[1];
            //    neighbour[0] = _route.First;
            //}
            //else if (_route.Last == customer)
            //{
            //    neighbour = new int[1];
            //    neighbour[0] = 0;
            //}
            return _route.GetNeigbours(customer);
        }

        /// <summary>
        /// Returns the index of the given customer the first being zero.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int GetIndexOf(int customer)
        {
            if (customer == 0)
            {
                return 0;
            }
            int idx = _route.GetIndexOf(customer);
            if (idx < 0)
            {
                return idx;
            }
            return idx + 1;
        }

        /// <summary>
        /// Returns true if the route is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (_route == null)
            {
                return true;
            }
            return _route.IsValid() && !_route.Contains(0);
        }

        /// <summary>
        /// Returns an enumerator between two customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IEnumerable<int> Between(int from, int to)
        {
            return new BetweenEnumerable(this, from, to);
        }

        /// <summary>
        /// Returns an enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a round the pair that contains last->first is also included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pair> Pairs()
        {
            if (_route == null)
            {
                return (new List<Pair>());
            }
            return new PairEnumerable(this);
        }

        /// <summary>
        /// Returns an enumerable that enumerates all customer triples that occur in the route as 1->2-3. If the route is a round the tuples that contain last->first are also included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Triple> Triples()
        {
            if (_route == null)
            {
                return (new List<Triple>());
            }
            return new TripleEnumerable(this);
        }

        /// <summary>
        /// Returns an enumerator of all customers.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator()
        {
            return new DepotRouteEnumerator(_route.GetEnumerator());
        }

        /// <summary>
        /// Returns an enumerator of all customers.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new DepotRouteEnumerator(_route.GetEnumerator());
        }

        /// <summary>
        /// Clones the route.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            IRoute cloned_route = _route.Clone() as IRoute;
            return new DepotRoute(cloned_route);
        }

        /// <summary>
        /// Returns a description of this depot route.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            int previous = -1;
            StringBuilder result = new StringBuilder();
            foreach (int customer in this)
            {
                if (previous < 0)
                {
                    result.Append(customer);
                }
                else
                {
                    result.Append("->");
                    result.Append(customer);
                }
                previous = customer;
            }
            return result.ToString();
        }

        /// <summary>
        /// An enumerable for all customers in this route.
        /// </summary>
        private class DepotRouteEnumerable : IEnumerable<int>
        {
            private IRoute _route;

            public DepotRouteEnumerable(IRoute route)
            {
                _route = route;
            }

            public IEnumerator<int> GetEnumerator()
            {
                return new DepotRouteEnumerator(_route.GetEnumerator());
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new DepotRouteEnumerator(_route.GetEnumerator());
            }
        }

        private class DepotRouteEnumerator : IEnumerator<int>
        {
            private IEnumerator<int> _route;

            private bool _depot = false;

            private int _customer = -1;

            public DepotRouteEnumerator(IEnumerator<int> route)
            {
                _route = route;
            }

            public int Current
            {
                get { return _customer; }
            }

            public void Dispose()
            {

            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (!_depot)
                {
                    _customer = 0;
                    _depot = true;
                    return true;
                }
                else
                {
                    if (_route.MoveNext())
                    {
                        _customer = _route.Current;
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                _depot = false;
                _route.Reset();
            }
        }

        /// <summary>
        /// Inserts a new first customer.
        /// </summary>
        /// <param name="first"></param>
        public void InsertFirst(int first)
        {
            throw new NotSupportedException("The first customer in a depot route cannot be changed, it always is the depot!");
        }

        /// <summary>
        /// Replaces the first customer.
        /// </summary>
        /// <param name="first"></param>
        public void ReplaceFirst(int first)
        {
            throw new NotSupportedException("The first customer in a depot route cannot be changed, it always is the depot!");
        }

        /// <summary>
        /// Clears all customer.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException("A depot route cannot be cleared. It always contains at least the depot!");
        }

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer">The customer to remove.</param>
        /// <param name="after">The customer that used to exist after.</param>
        /// <param name="before">The customer that used to exist before.</param>
        /// <returns></returns>
        public bool Remove(int customer, out int before, out int after)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shifts the given customer to a new location and places it after the given 'before' customer.
        /// </summary>
        /// <param name="customer">The customer to shift.</param>
        /// <param name="before">The new customer that will come right before.</param>
        /// <returns></returns>
        public bool ShiftAfter(int customer, int before)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shifts the given customer to a new location and places it after the given 'before' customer.
        /// </summary>
        /// <param name="customer">The customer to shift.</param>
        /// <param name="before">The new customer that will come right before.</param>
        /// <param name="oldBefore">The customer that used to exist before.</param>
        /// <param name="oldAfter">The customer that used to exist after.</param>
        /// <param name="newAfter">The customer that new exists after.</param>
        /// <returns></returns>
        public bool ShiftAfter(int customer, int before, out int oldBefore, out int oldAfter, out int newAfter)
        {
            throw new NotImplementedException();
        }
    }
}