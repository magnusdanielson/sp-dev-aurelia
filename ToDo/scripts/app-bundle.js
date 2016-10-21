define('todo',["require", "exports"], function (require, exports) {
    "use strict";
    var Todo = (function () {
        function Todo(description) {
            this.description = description;
            this.done = false;
        }
        return Todo;
    }());
    exports.Todo = Todo;
});

define('app',["require", "exports", './todo'], function (require, exports, todo_1) {
    "use strict";
    var App = (function () {
        function App() {
            this.message = 'Hello World!';
            this.heading = 'Todos';
            this.todos = [];
            this.todoDescription = '';
        }
        App.prototype.addTodo = function () {
            if (this.todoDescription) {
                this.todos.push(new todo_1.Todo(this.todoDescription));
                this.todoDescription = '';
            }
        };
        App.prototype.removeTodo = function (todo) {
            var index = this.todos.indexOf(todo);
            if (index !== -1) {
                this.todos.splice(index, 1);
            }
        };
        return App;
    }());
    exports.App = App;
});

define('environment',["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = {
        debug: true,
        testing: true
    };
});

define('main',["require", "exports", './environment'], function (require, exports, environment_1) {
    "use strict";
    Promise.config({
        warnings: {
            wForgottenReturn: false
        }
    });
    function configure(aurelia) {
        aurelia.use
            .defaultBindingLanguage()
            .defaultResources()
            .feature('resources');
        if (environment_1.default.debug) {
            aurelia.use.developmentLogging();
        }
        if (environment_1.default.testing) {
            aurelia.use.plugin('aurelia-testing');
        }
        aurelia.start().then(function () { return aurelia.setRoot('app', document.body); });
    }
    exports.configure = configure;
});

define('my-feature/index',["require", "exports"], function (require, exports) {
    "use strict";
    function configure(config) {
    }
    exports.configure = configure;
});

define('resources/index',["require", "exports"], function (require, exports) {
    "use strict";
    function configure(config) {
    }
    exports.configure = configure;
});

define('text!app.html', ['module'], function(module) { module.exports = "<template>\n  <h1>${heading}</h1>\n\n  <form submit.trigger=\"addTodo()\">\n    <input type=\"text\" value.bind=\"todoDescription\">\n    <button type=\"submit\">Add Todo</button>\n  </form>\n\n<ul>\n  <li repeat.for=\"todo of todos\">\n    <input type=\"checkbox\" checked.bind=\"todo.done\">\n    <span css=\"text-decoration: ${todo.done ? 'line-through' : 'none'}\">\n      ${todo.description}\n    </span>\n    <button click.trigger=\"removeTodo(todo)\">Remove</button>\n</template>\n"; });
//# sourceMappingURL=app-bundle.js.map