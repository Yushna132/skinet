//Par défaut message va etre un string et isComplete un boolean
var message = 'Hello';
var isComplete = false;
//Ici on a définit message2 comme un string
var message2 = 'Hello';
//Ici on a définit message3 soit comme un string ou un number
var message3 = 2;
//todos est un array de Todo
var todos = [];
//On va créer une fonction
//Cette fonction va retoruner un Todo
//En entrée on lui donner un string title
//On aurait pu ecrire title :title
function addTodo(title) {
    var newTodo = {
        id: todos.length + 1,
        title: title,
        completed: false,
    };
    todos.push(newTodo);
    return newTodo;
}
function toggleTodo(id) {
    var todo = todos.find(function (todo) { return todo.id === id; });
    if (todo) {
        todo.completed = !todo;
    }
}
addTodo('Build API');
addTodo('Publish app');
toggleTodo(1);
console.log(todos);
