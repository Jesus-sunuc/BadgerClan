using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadgerClan.MauiController.Models;


public interface IEndpointItem
{
    string DisplayName { get; }
    Task SetStrategyAsync(string strategyName);
}