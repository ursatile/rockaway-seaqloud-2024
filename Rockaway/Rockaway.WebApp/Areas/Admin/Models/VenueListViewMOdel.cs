using Rockaway.WebApp.Data.Entities;

public class VenueListViewModel {
	public VenueListViewModel(IEnumerable<Venue> venues) {
		this.Venues = venues;
	}
	public bool AllowCreate { get; set; }
	public bool AllowView { get; set; }
	public bool AllowEdit { get; set; }
	public IEnumerable<Venue> Venues { get; set; }
}