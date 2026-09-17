const response = await fetch("/api/message");
const message = await response.text();

function App() {
  return <h1>{message}</h1>;
}

export default App;
