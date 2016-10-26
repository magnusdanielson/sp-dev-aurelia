import {RouterConfiguration, Router} from 'aurelia-router';

export class App {
    currentRouter: Router;
  configureRouter(config: RouterConfiguration, router: Router): void {
    this.currentRouter = router;
    config.title = 'Aurelia';
    config.map([
      { route: ['', 'todos'], name: 'todos',      moduleId: './todos',      nav: true, title: 'Todos' },
      { route: 'binding', name: 'binding',      moduleId: './binding-demo',      nav: true, title: 'Binding' },
      { route: 'users',         name: 'users',        moduleId: './users',        nav: true, title: 'Github Users' },
      { route: 'sp',         name: 'sp',        moduleId: './sp-title',        nav: true, title: 'SharePoint' },
      { route: 'child-router',  name: 'child-router', moduleId: './child-router', nav: true, title: 'Child Router' }
    ]);

  }
}