using System;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
	[ApiController]
	[Route("api/auctions")]
	public class AuctionsControllers : ControllerBase
	{
		private readonly AuctionDbContext _context;
		private readonly IMapper _mapper;

		public AuctionsControllers(AuctionDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}


		[HttpGet]
		public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions()
		{
			var auctions = await _context.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();
			return _mapper.Map<List<AuctionDTO>>(auctions);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
		{
			var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

			if (auction == null) return NotFound();

			return _mapper.Map<AuctionDTO>(auction);
		}

		[HttpPost]
		public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO crteateauctionDTO)
		{
			var auction = _mapper.Map<Auction>(crteateauctionDTO);
			_context.Auctions.Add(auction);
			var result = await _context.SaveChangesAsync() > 0;

			if (!result) return BadRequest("Could not save the cahnges to the DB");

			return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDTO>(auction));


        }

		[HttpPut("{id}")]
		public async Task<ActionResult>UpdateAuction(Guid id,UpdateAuctionDTO updateAuctionDTO)
		{
			var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

			if (auction == null) return NotFound();

			//TODO: check seller == userName.

			auction.Item.Make = updateAuctionDTO.Make ?? auction.Item.Make;
			auction.Item.Model = updateAuctionDTO.Model ?? auction.Item.Model;
			auction.Item.Mileage = updateAuctionDTO.Mileage ?? auction.Item.Mileage;
			auction.Item.Year = updateAuctionDTO.Year ?? auction.Item.Year;
			auction.Item.Color = updateAuctionDTO.Color ?? auction.Item.Color;

			var result = await _context.SaveChangesAsync() > 0;

			if (!result) return BadRequest("Failed to update data in db");

			return Ok();


		}
	}
}

