﻿//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Core.Contracts;
using Naif.Data.ComponentModel;
using NPoco;
// ReSharper disable UseStringInterpolation

namespace Naif.Data.NPoco
{
    public class NPocoRepository<T> : RepositoryBase<T> where T : class
    {
        private readonly Database _database;

        public NPocoRepository(Database database, ICacheProvider cache)
            : this(database, cache, new NPocoMapper(String.Empty))
        {
        }

        public NPocoRepository(Database database, ICacheProvider cache, IMapper mapper)
            : base(cache)
        {
            Requires.NotNull("database", database);

            _database = database;

            _database.Mapper = mapper;
        }

        public override IEnumerable<T> Find(string sqlCondition, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override IPagedList<T> Find(int pageIndex, int pageSize, string sqlCondition, params object[] args)
        {
            throw new NotImplementedException();
        }

        protected override void AddInternal(T item)
        {
            _database.Insert(item);
        }

        protected override void DeleteInternal(T item)
        {
            _database.Delete(item);
        }

        protected override IEnumerable<T> GetAllInternal()
        {
            return _database.Fetch<T>("");
        }

        protected override T GetByIdInternal<TProperty>(TProperty id)
        {
            return _database.SingleOrDefault<T>(String.Format("WHERE {0} = @0", Util.GetPrimaryKeyName(typeof(T).GetTypeInfo())), id);
        }

        protected override IEnumerable<T> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            return _database.Query<T>(String.Format("WHERE {0} = @0", propertyName), propertyValue);
        }

        protected override IEnumerable<T> GetByScopeInternal(object propertyValue)
        {
            throw new NotImplementedException();
        }

        protected override IPagedList<T> GetPageInternal(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        protected override IPagedList<T> GetPageByScopeInternal(object propertyValue, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateInternal(T item)
        {
            _database.Update(item);
        }
    }
}
