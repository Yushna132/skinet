//Par défaut message va etre un string et isComplete un boolean

let message = 'Hello';
let isComplete = false;

//Ici on a définit message2 comme un string
let message2: string = 'Hello';

//Ici on a définit message3 soit comme un string ou un number
let message3: string | number = 2;

//un type c'est un interface.

interface TodoInterface {
  //on utilise ? pour faire id devenir optionelle
  id?: number;
  title: string;
  completed: boolean;
}

type Todo = {
  id: number;
  title: string;
  completed: boolean;
};

//todos est un array de Todo
let todos: Todo[] = [];

//On va créer une fonction
//Cette fonction va retoruner un Todo
//En entrée on lui donner un string title
//On aurait pu ecrire title :title
function addTodo(title: string): Todo {
  const newTodo: Todo = {
    id: todos.length + 1,
    title,
    completed: false,
  };
  todos.push(newTodo);
  return newTodo;
}

function toggleTodo(id: number): void {
  const todo = todos.find((todo) => todo.id === id);
  if (todo) {
    todo.completed = !todo;
  }
}

addTodo('Build API');
addTodo('Publish app');
toggleTodo(1);

console.log(todos);
