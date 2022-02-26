import { render, screen } from '@testing-library/react';
import userEvent from "@testing-library/user-event";
import { BrowserRouter } from 'react-router-dom';
import App from '../App';

test('Renders navigation link', () => {
    render(
        <BrowserRouter>
            <App />
        </BrowserRouter>
    );

    const navigationElement = screen.getByText(/Audit Log/i);
    expect(navigationElement).toBeInTheDocument();
});

test('Navigation link becomes active on click', () => {
    render(
      <BrowserRouter>
          <App />
      </BrowserRouter>
    );

    const navigationElement = screen.getByText(/Audit Log/i);
    userEvent.click(navigationElement);

    expect(navigationElement).toHaveClass('active');
});