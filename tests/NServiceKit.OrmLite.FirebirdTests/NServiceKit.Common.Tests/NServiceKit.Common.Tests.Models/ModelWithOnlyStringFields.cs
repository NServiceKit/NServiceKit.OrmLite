﻿using System;

namespace NServiceKit.Common.Tests.Models{

    /// <summary>A model with only string fields.</summary>
	public class ModelWithOnlyStringFields
	{
        /// <summary>Gets or sets the identifier of the album.</summary>
        /// <value>The identifier of the album.</value>
		public string AlbumId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the album.</summary>
        /// <value>The name of the album.</value>
		public string AlbumName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public string Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public string Name
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithOnlyStringFields
        /// class.
        /// </summary>
		public ModelWithOnlyStringFields()
		{
		}

        /// <summary>Creates a new ModelWithOnlyStringFields.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The ModelWithOnlyStringFields.</returns>
		public static ModelWithOnlyStringFields Create(string id)
		{
			ModelWithOnlyStringFields modelWithOnlyStringField = new ModelWithOnlyStringFields();
			modelWithOnlyStringField.Id = id;
			modelWithOnlyStringField.Name = "Name";
			modelWithOnlyStringField.AlbumId = "AlbumId";
			modelWithOnlyStringField.AlbumName = "AlbumName";
			return modelWithOnlyStringField;
		}
	}
}