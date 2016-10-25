import {Aurelia} from 'aurelia-framework';
import {Todo} from './todo';


export class App {
  heading: string;
  todos: Array<Todo>;
  todoDescription: string;
  message = 'Hello World!';
  userName: string;
  
  constructor()
  {
    this.heading = 'Todos';
    this.todos = [];
    this.todoDescription = '';
    if(typeof userName != 'undefined')
      {
          this.userName = userName;
      }
      else
      {
          this.userName = "inte än."
      }
  }

  addTodo()
  {
    if(this.todoDescription)
    {
      this.todos.push( new Todo(this.todoDescription));
      this.todoDescription = '';
    }
  }

  removeTodo(todo)
  {
    let index = this.todos.indexOf(todo);
    if(index !== -1)
    {
      this.todos.splice(index,1);
    }
  }

  

}
