// using AStar.Dev.Web.Shared;
// using JetBrains.AStar.Dev.Annotations;
// using Microsoft.AspNetCore.Components;
//
// namespace AStar.Dev.Web.UI.Shared;
//
// [TestSubject(typeof(NavMenu))]
// public sealed class NavMenuShould
// {
//     [Fact]
//     public void ImplementIDisposable()
//     {
//         var menu = new NavMenuForTests { NavigationManager = new TestNav(), };
//
//         menu.ShouldBeAssignableTo<IDisposable>();
//     }
//
//     [Fact]
//     public void DisposeOfTheNavigationManager()
//     {
//         var navigationManager = new TestNav();
//         var menu    = new NavMenuForTests { NavigationManager = navigationManager, };
//         navigationManager.LocationChanged += (s, e) => { };
//
//         menu.Dispose();
//
//         menu.NavigationManager.ShouldBeNull();
//     }
//
//     [Fact]
//     public void TriggerTheLocationChangedEvent()
//     {
//         var count   = 0;
//         var navigationManager = new TestNav();
//         var menu    = new NavMenuForTests { NavigationManager = navigationManager, };
//         navigationManager.LocationChanged += (s, e) => { count++; };
//
//         navigationManager.NavigateTo("https://www.example.com");
//         count.ShouldBe(1);
//     }
// }
//
// internal sealed class NavMenuForTests : NavMenu;
//
// internal sealed class TestNav : NavigationManager
// {
//     public TestNav() =>
//         Initialize("https://unit-test.example/", "https://unit-test.example/");
//
//     protected override void NavigateToCore(string uri, bool forceLoad) =>
//         NotifyLocationChanged(false);
// }

