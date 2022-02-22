import './styles/App.scss';
import { useEffect, useState } from 'react';

function App() {
  const [timestamp, setTimestamp] = useState(null);
  const [bids, setBids] = useState([]);
  const [asks, setAsks] = useState([]);

  useEffect(() => {
    fetch('https://localhost:7117/api/getorderbook?currencypair=btceur')
      .then(response => {
        if (response.ok) {
          return response.json();
        }

        throw response;
      })
      .then(data => {
        console.log(data);

        setTimestamp(data.timestamp);
        setBids(data.bids);
        setAsks(data.asks);
      })
      .catch(error => {
        console.error(`Error fetching data: ${error}`);
      });
  }, []);

  return (
    <div className="App d-flex flex-column">
      <p>Timestamp: {timestamp}</p>

      <div className='d-flex flex-row justify-content-center'>
        <table className='d-flex flex-column'>
          <thead>
            <tr>
              <th colSpan="2">
                Bids
              </th>
            </tr>
            <tr>
              <th>Price [EUR]</th>
              <th>Amount [BTC]</th>
            </tr>
          </thead>

          <tbody>
            {bids && bids.map((bid, index) => (
              <tr key={index}>
                <td>{bid[0]}</td>
                <td>{bid[1]}</td>
              </tr>
            ))}
          </tbody>
        </table>

        <table className='d-flex flex-column'>
          <thead>
            <tr>
              <th colSpan="2">
                Asks
              </th>
            </tr>
            <tr>
              <th>Price [EUR]</th>
              <th>Amount [BTC]</th>
            </tr>
          </thead>

          <tbody>
            {asks && asks.map((ask, index) => (
              <tr key={index}>
                <td>{ask[0]}</td>
                <td>{ask[1]}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default App;
