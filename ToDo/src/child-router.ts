import {Router, RouterConfiguration} from 'aurelia-router';

export class ChildRouter {
  router: Router;

  heading = 'Child Router';

  configureRouter(config: RouterConfiguration, router: Router) {
    config.map([
     { route: ['', 'todos'], name: 'todos',      moduleId: './todos',      nav: true, title: 'Todos' },
      { route: 'binding', name: 'binding',      moduleId: './binding-demo',      nav: true, title: 'Binding' },
      { route: 'users',         name: 'users',        moduleId: './users',        nav: true, title: 'Github Users' },
      { route: 'child-router',  name: 'child-router', moduleId: './child-router', nav: true, title: 'Child Router' }
    ]);

    this.router = router;
  }
}
