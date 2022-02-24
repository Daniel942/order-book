import './styles/App.scss';

import { Routes, Route } from 'react-router-dom';

import Navigation from './components/Navigation';
import OrderBook from './components/OrderBook';
import AuditLog from './components/AuditLog';
import Trade from './components/Trade';

function App() {
  return (
    <div className='App d-flex flex-column'>
      <Navigation />

      <Routes>
        <Route path='order-book' element={<OrderBook />} />
        <Route path='order-book/:id' element={<OrderBook />} />
        <Route path='audit-log' element= {<AuditLog />} />
        <Route path='trade' element= {<Trade />} />
      </Routes>
    </div>
  );
}

export default App;
