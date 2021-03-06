﻿using System;
using System.Collections.Generic;
using DotVVM.Framework.Compilation.Parser.Dothtml.Parser;
using System.Linq;

namespace DotVVM.Framework.Compilation.ControlTree.Resolved
{
    public class ResolvedTreeRoot : ResolvedContentNode, IAbstractTreeRoot
    {
        public Dictionary<string, List<IAbstractDirective>> Directives { get; set; } = new Dictionary<string, List<IAbstractDirective>>(StringComparer.OrdinalIgnoreCase);

        public ResolvedTreeRoot(ControlResolverMetadata metadata, DothtmlNode node, DataContextStack dataContext, IReadOnlyDictionary<string, IReadOnlyList<IAbstractDirective>> directives)
            : base(metadata, node, dataContext)
        {
            Directives = directives.ToDictionary(d => d.Key, d => d.Value.ToList());
            directives.SelectMany(d => d.Value).ToList().ForEach(d => { ((ResolvedDirective)d).Parent = this; });
        }

        public override void AcceptChildren(IResolvedControlTreeVisitor visitor)
        {
            foreach (var dir in Directives.Values)
            {
                dir.ForEach(d => (d as ResolvedDirective)?.Accept(visitor));
            }

            base.AcceptChildren(visitor);
        }

        public override void Accept(IResolvedControlTreeVisitor visitor)
        {
            visitor.VisitView(this);
        }
    }
}
