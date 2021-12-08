// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Augurk.Api
{
    /// <summary>
    /// A container for performed migrations.
    /// </summary>
    public class DbMigrations 
    {
        internal const string ID = "CompletedDataMigrations";

        /// <summary>
        /// Indicates whether the product from features migration has taken place.
        /// </summary>
        public bool ProductFromFeatures {get;set;}
    }
}
