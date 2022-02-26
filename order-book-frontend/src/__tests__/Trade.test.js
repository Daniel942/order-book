import { render, screen } from '@testing-library/react';
import Trade from '../components/Trade';

test('Renders buy BTC', () => {
    render(
        <Trade />
    );

    const navigationElement = screen.getByText(/Buy BTC/i);
    expect(navigationElement).toBeInTheDocument();
});