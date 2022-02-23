import './styles/App.scss';
import { useEffect, useState } from 'react';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

function App() {
  const [timestamp, setTimestamp] = useState(null);
  const [bids, setBids] = useState([]);
  const [asks, setAsks] = useState([]);

  const [chartOptions, setChartOptions] = useState(null);

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

        setChartOptions({
          chart: {
            type: 'area'
          },
          title: {
            text: 'Order book'
          },
          legend: {
            enabled: false
          },
          tooltip: {
            formatter: function () {
              return `${this.series.name}: ${this.y} BTC at ${this.x} EUR`
            },
            valueDecimals: 8
          },
          plotOptions: {
            area: {
                fillOpacity: 0.3,
                lineWidth: 1,
                step: 'center'
            }
          },
          series: [
            {
              name: 'Bids',
              data: data.bids.map(bid => bid.map(v => parseFloat(v))).reverse(),
              color: '#1F8787'
            }, {
            name: 'Asks',
            data: data.asks.map(ask => ask.map(v => parseFloat(v))),
            color: '#DB3E3D'
          }]
        });
      })
      .catch(error => {
        console.error(`Error fetching data: ${error}`);
      });
  }, []);

  return (
    <div className="App d-flex flex-column">
      <p>Timestamp: {timestamp}</p>

      {
        chartOptions && <HighchartsReact
          highcharts={Highcharts}
          options={chartOptions}
        />
      }

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
